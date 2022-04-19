namespace PuppyData.SqlMapper

module LookupValuesFromSql =
    open System.Collections.Generic
    open System
    open System.Data
    open System.Data.SqlClient
    open PuppyData.Types
    open Puppy.SqlMapper
    
    let private paramsQry = "
    SELECT UdfName, [SpForSearch], [SpForValidation]
	from Puppy.[LookupValueConstraints]
    "
    let inline (>?) a b =
        if isNull(a) then b else a.ToString()
    let private toValuePair (searchSp:string, validationSp: string) =
         LkupProcInfo {SpForSearch = searchSp; SpForValidation = validationSp} 

    let private getPossibleLookupsInfo (con) =
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
                            |> Array.head
                            |> (fun r ->  toValuePair(r.GetString("SpForSearch") , r.GetString("SpForValidation")) )
                    }
                ) 
            dr.Close()
            return infoes
        } 
    let mutable private cache = None

    let GetAllPossibleLookupsInfo con =
        async {
            match cache with
            | Some r -> return r
            | None ->
                let! ranges = getPossibleLookupsInfo con
                cache <- Some ranges
                return ranges
        }