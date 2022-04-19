namespace PuppyData.SqlMapper

module PossibleValuesFromSql =
    open System.Collections.Generic
    open System
    open System.Data
    open System.Data.SqlClient
    open PuppyData.Types
    open Puppy.SqlMapper
    
    let private paramsQry = "
    SELECT UdfName, [Value], [Label]
	from Puppy.[ValidValuesConstraints]
	order by [Label], [Value]
    "
    let inline (>?) a b =
        if isNull(a) then b else a.ToString()
    let private toValuePair (valueObj:Object, labelObj: Object): ValueLabelPair =
        let value = valueObj >? "" 
        let label = labelObj >? value
        { Value =value; Label = label} 

    let private getPossibleValuesInfo (con) =
        async {
            use cmd = new SqlCommand(paramsQry , con)
            cmd.CommandType <- CommandType.Text
            let! dr = cmd.ExecuteReaderAsync() |> Async.AwaitTask;
            
            let drSeq = dr |> SqlUtils.ConvertToDictionary
            let infoes =  
                drSeq 
                |> Array.groupBy (fun r -> r.["UdfName"].ToString())
                |> Array.map (
                    fun (key, vals) -> {
                        UdfName = key
                        PossibleValues = 
                            vals 
                            |> Array.map (fun r -> toValuePair(r.["Value"] , r.["Label"]) )
                            |> Array.toSeq
                            |> PossibleValues
                    }
                ) 
            dr.Close()
            return infoes
        } 
    let mutable private cache = None

    let GetAllPossibleValuesInfo con =
        async {
            match cache with
            | Some r -> return r
            | None ->
                let! ranges = getPossibleValuesInfo con
                cache <- Some ranges
                return ranges
        }