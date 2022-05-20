namespace PuppyData.SqlMapper

module StoredProcMetaData =
    open System.Collections.Generic
    open System
    open System.Data
    open System.Data.SqlClient
    open PuppyData.Types
    open Puppy.SqlMapper
    
    let private paramsQry = "
    SELECT *
	from [puppy].[StoredProceduresMetaData]
    "
    let inline (>?) a b =
        if isNull(a) then b else a.ToString()
    
    let private mapMetaData (r:IDataReader) = 
        let headings = [|
            r.["Heading1"].ToString()
            r.["Heading2"].ToString()
            r.["Heading3"].ToString()
            r.["Heading4"].ToString()
        |]
        let actionTypes = [|
            r.["ActionType1"].ToString()
            r.["ActionType2"].ToString()
            r.["ActionType3"].ToString()
            r.["ActionType4"].ToString()
        |]
        let actionUrlTemplates = [|
            r.["ActionURITemplate1"].ToString()
            r.["ActionURITemplate2"].ToString()
            r.["ActionURITemplate3"].ToString()
            r.["ActionURITemplate4"].ToString()
        |]
        let actionDataTemplates = [|
            r.["ActionDataTemplate1"].ToString()
            r.["ActionDataTemplate2"].ToString()
            r.["ActionDataTemplate3"].ToString()
            r.["ActionDataTemplate4"].ToString()
        |]
        {
            SpName = r.["SpName"].ToString()
            Headings =headings
            ActionTypes = actionTypes
            ActionURITemplates = actionUrlTemplates
            ActionDataTemplates = actionDataTemplates
        }
    let private getMetaDataForSpResults (con)  =
        async {
            use cmd = new SqlCommand(paramsQry , con)
            cmd.CommandType <- CommandType.Text
            try 
                let! dr = cmd.ExecuteReaderAsync() |> Async.AwaitTask;
                let infoes = 
                    seq {
                        while (dr.Read()) do
                            yield (mapMetaData dr)
                    } 
                    |> Seq.toList
                dr.Close()
                return infoes 
            with exn ->
                return List.empty
        } 
    let mutable private cache = None

    let GetAllResultsMeataData con =
        async {
            match cache with
            | Some r -> return r
            | None ->
                let! ranges = getMetaDataForSpResults con
                cache <- Some ranges
                return ranges
        }