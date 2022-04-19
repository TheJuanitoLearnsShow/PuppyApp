namespace Puppy.SqlMapper

module SqlUtils =
    open System.Data
    open System.Collections.Generic
    open System
    open System.Linq
    open Humanizer
    open PuppyData.Types
    open System.Data.SqlClient
    
    let NullIfDbnull (o:Object) =
        if DBNull.Value.Equals(o) then null else o

    let NoneIfDbnull (o:Object) =
        if DBNull.Value.Equals(o) then None else Some o
    
    let ConvertToDictionary(reader: IDataReader ) : 
        IDictionary<string, Object> array=
        let columns = new LinkedList<string>();
        let rows = new LinkedList<IDictionary<string, Object>>();

        for  i  in 0..(reader.FieldCount - 1) do
            columns.AddLast(reader.GetName(i))  |> ignore

        while (reader.Read()) do
            let dict: IDictionary<string, Object> = new Dictionary<string, Object>() :> IDictionary<string, Object>
            columns |> Seq.iter (fun columnName -> dict.Add(columnName, NullIfDbnull reader.[columnName] ) )
            rows.AddLast dict |> ignore
        rows.ToArray()

    let ConvertToDictionaryWithMetaData (reader: IDataReader ) : SqlExecResult =
        let rows = new LinkedList<IDictionary<string, Object>>();
        
        let columns = 
            Array.init reader.FieldCount (
                fun i ->
                    let colName = reader.GetName(i)
                    let typeName = reader.GetFieldType(i)
                    { 
                        Name =  colName
                        FriendlyName = colName.Humanize()
                        NetType = typeName.Name
                    }
            )
            

        while (reader.Read()) do
            let dict: IDictionary<string, Object> = new Dictionary<string, Object>() :> IDictionary<string, Object>
            columns |> Array.iter (fun column -> dict.Add(column.Name, NullIfDbnull reader.[column.Name] ) )
            rows.AddLast dict |> ignore
        
        let rows = rows.ToArray()
        (columns, rows)

    let ExecSpReader (conStr:string) (spName:string) (spParams:(string * Object) seq ) (mapLogic: IDataReader -> 'T) = 
        async {
            let rows = new LinkedList<'T>();
            use conn = new SqlConnection(conStr)
            conn.Open()
            use cmd = new SqlCommand(spName , conn)
            cmd.CommandType <- CommandType.StoredProcedure
            for (pName, pVal) in spParams do
                cmd.Parameters.AddWithValue(pName, pVal) |> ignore
            let! dr = cmd.ExecuteReaderAsync() |> Async.AwaitTask
            while (dr.Read()) do
                let newRow = mapLogic dr
                rows.AddLast(newRow) |> ignore
            dr.Close()
            conn.Close()
            return rows
        } 
    
    let ExecSpNonReader (conStr:string) (spName:string) (spParams:(string * Object) seq ) = 
        async {
            use conn = new SqlConnection(conStr)
            conn.Open()
            use cmd = new SqlCommand(spName , conn)
            cmd.CommandType <- CommandType.StoredProcedure
            for (pName, pVal) in spParams do
                cmd.Parameters.AddWithValue(pName, pVal) |> ignore
            let! res =  cmd.ExecuteNonQueryAsync() |> Async.AwaitTask
           
            conn.Close()
            return res
        } 

    let ExecuteSpMultiResults (conStr:string) (spName:string) (spParams:(string * Object) seq ) (logicMappers: (IDataReader -> 'T) list) =
            async {
                let datasets = new LinkedList<LinkedList<'T>>();
                use conn = new SqlConnection(conStr)
                conn.Open()
                use cmd = new SqlCommand(spName , conn)
                cmd.CommandType <- CommandType.StoredProcedure
                for (pName, pVal) in spParams do
                    cmd.Parameters.AddWithValue(pName, pVal) |> ignore
                let! dr = cmd.ExecuteReaderAsync() |> Async.AwaitTask

                let mutable restOfMappers = logicMappers
                let mutable recordSetIdx = 1
                while (dr.HasRows) do
                    match restOfMappers with
                    |  (mapLogic::restOfMappers') ->
                        let rows = new LinkedList<'T>();
                        restOfMappers <- restOfMappers'
                        while (dr.Read()) do
                            let newRow = mapLogic dr
                            rows.AddLast(newRow) |> ignore
                        datasets.AddLast(rows) |> ignore
                        recordSetIdx <- recordSetIdx + 1
                        dr.NextResult() |> ignore
                    | ([]) ->
                        failwithf "No mappers available for %d " recordSetIdx
                dr.Close()
                return datasets.ToArray()
                 

            }