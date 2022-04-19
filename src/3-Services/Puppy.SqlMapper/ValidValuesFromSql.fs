namespace PuppyData.SqlMapper

module ValidValuesFromSql =
    open System.Collections.Generic
    open System
    open System.Data
    open System.Data.SqlClient
    open PuppyData.Types
    open Puppy.SqlMapper
    
    let private paramsQry = "
    select  
   [UdfName]
   ,[From]
   ,[To]
   ,CustomMessage
  from puppy.[ValidRangeConstraints]
  order by UdfName, id
    "
    
    let private toRangeType (fromValue:obj , toValue: obj, msg: obj) =
        let fromValueT = if DBNull.Value.Equals(fromValue) || isNull(fromValue) then None else Some (fromValue.ToString())
        let toValueT = if DBNull.Value.Equals(toValue) || isNull(toValue) then None else Some (toValue.ToString())
        let msgT = if DBNull.Value.Equals(msg) || isNull(msg) then None else Some (msg.ToString())
        match (fromValueT, toValueT, msgT) with
        | (Some f, None, m) -> 
            LowerLimit (f,m)
        | (Some f, Some t, m)  -> 
            BetweenLimit (f, t, m)
        | (None, Some t, m)  -> 
            UpperLimit (t, m)

    let private getValidRanges (con) =
        async {
            use cmd = new SqlCommand(paramsQry , con)
            cmd.CommandType <- CommandType.Text
            let! dr = cmd.ExecuteReaderAsync() |> Async.AwaitTask;
            
            let drSeq = dr |> SqlUtils.ConvertToDictionary
            let infoes =  
                drSeq 
                |> Array.groupBy (fun r -> r.["UdfName"].ToString())
                |> Array.map (
                    fun (key, ranges) -> {
                        UdfName = key
                        Ranges = 
                            ranges 
                            |> Array.map (fun r -> toRangeType(r.["From"] , r.["To"], r.["CustomMessage"]) )
                    }
                ) 
            dr.Close()
            return infoes
        } 
    let mutable private cacheRanges = None

    let GetAllValidRanges con =
        async {
            match cacheRanges with
            | Some r -> return r
            | None ->
                let! ranges = getValidRanges con
                cacheRanges <- Some ranges
                return ranges
        }