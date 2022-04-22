namespace PuppyData.SqlMapper

module LookupValuesFromSql =
    open System.Collections.Generic
    open System
    open System.Data
    open System.Data.SqlClient
    open PuppyData.Types
    open Puppy.SqlMapper
    
    let private paramsQry = "
    SELECT UdfName, [ObjectForSearch], [SearchParameterName], [IdColumnName], [LabelColumnName], [IsStoredProc]
	from puppy.[LookupValueConstraints]
    "
    let inline (>?) a b =
        if isNull(a) then b else a.ToString()
    
    let private getPossibleLookupsInfo (con) =
        async {
            use cmd = new SqlCommand(paramsQry , con)
            cmd.CommandType <- CommandType.Text
            try 
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
                                |> (fun r ->  
                                 LkupInfo { 
                                    ObjectForSearch = r.["ObjectForSearch"].ToString()
                                    SearchParameterName= r.["SearchParameterName"].ToString()
                                    IdColumnName= r.["IdColumnName"].ToString()
                                    LabelColumnName= r.["LabelColumnName"].ToString()
                                    IsStoredProc= (r.["IsStoredProc"] = true)
                                })
                        }
                    ) 
                dr.Close()
                return infoes
            with exn ->
                return Array.empty
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