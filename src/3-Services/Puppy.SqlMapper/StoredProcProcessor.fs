namespace PuppyData.SqlMapper

open System.Data.SqlClient

module StoredProcProcessor =

    open System.Collections.Concurrent
    open FsCommon
    open FsCommon.RangeValidations
    open PuppyData.Types
    open System
    open FsCommon.Validations
    open Puppy.SqlMapper
    open System.Collections.Generic
    open System.Data.SqlClient
    open System.Data
    open Humanizer
    let private CreateBaseRangeFor (typeInfo: PuppySqlTypeInfo) =
        let maxNum =  Array.create (typeInfo.Length - typeInfo.Decimals) "9" |>  String.concat ""
        let decimalDigits =  Array.create (typeInfo.Decimals) "9" |>  String.concat ""
        let m = Decimal.Parse (maxNum  + "." + decimalDigits)
        RangeType.BetweenLimit (((-m).ToString()),(m.ToString()),None)
    let CreateValueForNumeric (typeInfo: PuppySqlTypeInfo) (value: string) : Result<Object, string> =
        let nature = SqlTypeTranslator.GetNature typeInfo.Nature
        let validateRange = (isInRange nature)
        let baseRange = CreateBaseRangeFor typeInfo
        value
        |> isFilled typeInfo.Required
        |> canItBeConvertedFromtext typeInfo
        >>= HasCorrectTotalDecimalDigits typeInfo.Decimals
        >>= nature.FromText
        |> collectionMapPass typeInfo.Ranges validateRange 
        >>= (fun o -> isInRange nature o baseRange) 


    
    let CreateValue (typeInfo: PuppySqlTypeInfo) (value: string) : Result<Object, string> =
        let nature = SqlTypeTranslator.GetNature typeInfo.Nature
        let validateRange = (isInRange nature)
        value
        |> isFilled typeInfo.Required
        |> canItBeConvertedFromtext typeInfo
        >>= isCorrectLenght 0 typeInfo.Length
        >>= nature.FromText
        |> collectionMapPass typeInfo.Ranges validateRange 

    let CreateValueDate (typeInfo: PuppySqlTypeInfo) (value: string) : Result<Object, string> =
        let nature = SqlTypeTranslator.GetNature typeInfo.Nature
        let validateRange = (isInRange nature)
        value
        |> isFilled typeInfo.Required
        >>= nature.FromText
        |> collectionMapPass typeInfo.Ranges validateRange

    let private valueCreators = dict [
        typeof<decimal>.Name, CreateValueForNumeric
        typeof<int>.Name, CreateValueForNumeric
        typeof<DateTime>.Name, CreateValueDate
        typeof<string>.Name, CreateValue
    ]
    let private cacheParamHelpers = new ConcurrentDictionary<string, ParamHelper seq>()
    
        
    let GetParamHelpers con spName = 
        async {
            if cacheParamHelpers.ContainsKey spName then 
                return cacheParamHelpers.[spName]
            else
                let! ranges = ValidValuesFromSql.GetAllValidRanges con 
                let! lkupInfo = 
                    [| PossibleValuesFromSql.GetAllPossibleValuesInfo con ;   
                        LookupValuesFromSql.GetAllPossibleLookupsInfo con
                        |]
                        |> Async.Parallel
                        
                let findRanges = (
                    fun typeName -> 
                        ranges 
                        |> Seq.tryFind (fun r -> String.Compare(r.UdfName , typeName, true) = 0 ) 
                        |> Option.map (fun r -> r.Ranges)
                        |> Option.defaultValue Seq.empty
                        )
                let mapSpParamToPuppy = StoredProcInfo.MapSpParamToPuppy findRanges (lkupInfo |> Array.concat)
                let! paramsInfo = StoredProcInfo.GetInputParams con spName 
                let spParamHelpers = 
                    paramsInfo 
                    |> Seq.map (fun p -> 
                        {
                            SpParamName = p.ParameterName
                            PuppyInfo = mapSpParamToPuppy  p
                            FriendlyName = p.ParameterName.Humanize()
                        }
                    
                    )
                    |> Seq.toArray
                cacheParamHelpers.[spName] <- spParamHelpers
                return spParamHelpers |> Array.toSeq
        }

    let ValidateSpParam h pValueStr =
        let createFunc = valueCreators.[h.PuppyInfo.Nature]
        pValueStr
        |> createFunc h.PuppyInfo 
        |> Result.mapError (fun e -> {
                ErrorProperty = h.SimpleName()
                ErrorMessage = [| e |]
            })
        |> Result.map (fun v -> (h.SpParamName, v))

    let private buildSpParamsValues (spParamHelpers:ParamHelper seq) (spParamValue:IDictionary<string,string>) =
        //TODO should we doing the caching here?
        spParamHelpers 
        |> Seq.map (fun h -> 
            let createFunc = valueCreators.[h.PuppyInfo.Nature]
            let pValueStr = spParamValue.[h.SpParamName.Substring(1)]
            pValueStr
            |> createFunc h.PuppyInfo 
            |> Result.mapError (fun e -> {
                    ErrorProperty = h.SimpleName()
                    ErrorMessage = [| e |]
                })
            |> Result.map (fun v -> (h.SpParamName, v))
            )
        |> Seq.toArray

    let (|Integer|_|) (str: string) =
       let mutable intvalue = 0
       if System.Int32.TryParse(str, &intvalue) then Some(intvalue)
       else None
    
    
    let (|ErrorData|ResultsData|) (metaData: ResultsMetadata) =
        if metaData.Length > 0 && metaData.[0].Name = Helpers.StandardErrorPropName then
            ErrorData 
        else
            ResultsData 
    
    

    let private runSp (cmd: SqlCommand) =
        async {
                try
                    let! dr = cmd.ExecuteReaderAsync() |> Async.AwaitTask;
                    let (meta, data) = dr |> SqlUtils.ConvertToDictionaryWithMetaData 
                    dr.Close() 
                    match meta with
                    | ErrorData ->
                        return { 
                            Results = Array.empty
                            MetaData = meta
                            Errors = Helpers.ToErrorRendition data
                        }
                    | ResultsData ->
                        return { 
                            Results = [| data |]
                            MetaData = meta
                            Errors = Seq.empty
                        }
                with
                | ex -> return Helpers.SpResultWithError ex.Message
        }
      
    let ValidateSpParams con (spName: string) (spParamValue:IDictionary<string,string>) =
        //TODO should we doing the caching here?
        async {
            let! spParamHelpers = GetParamHelpers con spName 
            let valuesResults = buildSpParamsValues spParamHelpers spParamValue
            let errors = valuesResults |> ChooseErrors
            return errors
        }

    let ExecuteSp con (spName: string) (spParamValue:IDictionary<string,string>) =
        //TODO should we doing the caching here?
        async {
            let! spParamHelpers = GetParamHelpers con spName 
            let valuesResults = buildSpParamsValues spParamHelpers spParamValue
            let errors = valuesResults |> ChooseErrors

            if errors |> Seq.isEmpty then
                
                let values = valuesResults |> ChooseSucesses 
                use cmd = new SqlCommand(spName , con)
                cmd.CommandType <- CommandType.StoredProcedure
                values |> Seq.iter (fun (sqlParamName, sqlParamValue) -> 
                    cmd.Parameters.AddWithValue( sqlParamName, sqlParamValue) |> ignore)
                return! runSp cmd
            else 
                return { 
                    Results = Array.empty
                    MetaData = Array.empty
                    Errors = errors 
                }
                

        }

    let ExecuteLookupSql (conStr:string) (lkpInfo: LookupInfo) (queryString:string) =
        task {
            let mapRow (dr:IDataReader) =
                {Value = dr.[lkpInfo.IdColumnName].ToString(); Label = dr.[lkpInfo.LabelColumnName].ToString() }
            if lkpInfo.IsStoredProc then
                let! rows = SqlUtils.ExecSpReaderAsTask conStr lkpInfo.ObjectForSearch [( "@" + lkpInfo.SearchParameterName, queryString)] mapRow
                return rows
            else
                //todo sanitize coluimn names
                let sqlQry = $"SELECT [{lkpInfo.IdColumnName}], [{lkpInfo.LabelColumnName}] from [{lkpInfo.ObjectForSearch}] where [{lkpInfo.SearchParameterName}] " + "like '%' + @QueryString + '%'"
                let! rows = SqlUtils.ExecSqlTextReaderAsTask conStr sqlQry [( "@QueryString", queryString)] mapRow
                return rows
        }
        
    
    let ExecuteSpAsTask con (spName: string) (inputDict:IDictionary<string,string>) = ExecuteSp con spName inputDict |> Async.StartAsTask
    let GetParamHelpersAsTask con (spName: string) = GetParamHelpers con spName |> Async.StartAsTask
  
    let ExecuteSpTestAsTask con  =
        
        let inputDict: IDictionary<string, string> =
             dict[
                    "LastName", "lolo" 
                    "MiddleName", ""
                    "GradeLevel", "2"
                    "CurrGPA", "1.0"
                    "ShortName", "Pepe" 

                ]
        ExecuteSp con "spAddStudent" inputDict 
        |> Async.StartAsTask


