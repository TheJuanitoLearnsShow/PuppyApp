namespace Puppy.SqlMapper

open PuppyData.Types

module StoredProcInfo =
    open System.Collections.Generic
    open System.Data
    open PuppyData.Types
    open System.Data.SqlClient
    open FsCommon
    open System

    let private spParamsTypesQry = "
    select  
    name ParameterName,  
    type_name(user_type_id) TypeName,  
    type_name(system_type_id) BaseSqlTypeName,
    max_length MaxLen ,  
    case when type_name(system_type_id) = 'uniqueidentifier' 
                then precision  
                else OdbcPrec(system_type_id, max_length, precision) end Prec,  
    OdbcScale(system_type_id, scale) Scale, 
	case when OdbcScale(system_type_id, scale) is not null then cast(1 as bit)  else cast(0 as bit) end IsNumericType, 
    parameter_id ParamOrder,  
    convert(sysname, 
                    case when system_type_id in (35, 99, 167, 175, 231, 239)  
                    then ServerProperty('collation') end) Collation  

    from sys.parameters where object_id = object_id(@spName)
    "
    
    //let private CreateBaseRangeFor (typeInfo: SpParamInfo) =
    //    let maxNum =  Array.create (typeInfo.Prec - typeInfo.Scale) "9" |>  String.concat ""
    //    let decimalDigits =  Array.create (typeInfo.Scale) "9" |>  String.concat ""
    //    let m = Decimal.Parse (maxNum  + "." + decimalDigits)
    //    RangeType.BetweenLimit (((-m).ToString()),(m.ToString()),None)

    let MapSpParamToPuppy getRangeFor (possibleLookupValues: PossibleValuesInfo seq) (p:SpParamInfo) =
        let lkInfo = 
            possibleLookupValues 
            |> Seq.tryFind (fun v -> v.UdfName = p.TypeName)
            |> Option.map (fun o -> o.PossibleValues)
        if p.IsNumericType then 
            {
                Required = true
                Length = p.Prec
                Nature = p.BaseSqlTypeName |> Puppy.SqlMapper.SqlTypeTranslator.ToNature
                Decimals = p.Scale
                Ranges = getRangeFor p.TypeName
                LookupInfo = lkInfo
            }
        else 
            {
                Required = true
                Length = p.MaxLen
                Nature = p.BaseSqlTypeName |> Puppy.SqlMapper.SqlTypeTranslator.ToNature
                Decimals = 0
                Ranges = getRangeFor p.TypeName
                LookupInfo = lkInfo
            }
        
    let GetInputParams (con) spName =
        async {
            use cmd = new SqlCommand(spParamsTypesQry , con)
            cmd.CommandType <- CommandType.Text
            cmd.Parameters.AddWithValue("@spName", spName) |> ignore
            let! dr = cmd.ExecuteReaderAsync() |> Async.AwaitTask;
            let infoes =  
                dr 
                |> SqlUtils.ConvertToDictionary 
                |> Array.map (ReflectionUtils.TypedObjectFromFromDict<SpParamInfo>)
            dr.Close()
            return infoes
        } 
    