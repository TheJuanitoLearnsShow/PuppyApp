using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PuppySqlWrapper
{

    public static class SqlResultAsJsonProcessor
    {

        public static async Task WriteSqlStoredProcResultAsJsonAsync(string connStr, string spName, Utf8JsonWriter writer,
            JsonSerializerOptions options, SqlParameter[] sqlParams = null, string[] dataSetNames = null)
        {
            using var connection = new SqlConnection(connStr);
            SqlCommand command = new SqlCommand(spName, connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            if (sqlParams != null)
            {
                command.Parameters.AddRange(sqlParams);
            }
            connection.Open();

            using SqlDataReader reader = command.ExecuteReader();

            await WriteMultiDataSetAsJsonAsync(writer, reader, options, dataSetNames);
            reader.Close();
        }

        public static async Task WriteSqlQueryResultAsJsonAsync(string connStr, string sqlQuery, Utf8JsonWriter writer,
            JsonSerializerOptions options =null, SqlParameter[] sqlParams = null, string[] dataSetNames = null)
        {
            using var connection = new SqlConnection(connStr);
            SqlCommand command = new(sqlQuery, connection);
            if (sqlParams != null)
            {
                command.Parameters.AddRange(sqlParams);
            }
            connection.Open();

            using SqlDataReader reader = command.ExecuteReader();

            await WriteMultiDataSetAsJsonAsync(writer, reader, options, dataSetNames);
            reader.Close();
        }

        // adapted from https://stackoverflow.com/a/59780795/217323 and https://stackoverflow.com/a/34927336/217323
        public static async Task WriteMultiDataSetAsJsonAsync(Utf8JsonWriter writer, SqlDataReader reader,
                    JsonSerializerOptions options, string[] dataSetNames)
        {

            writer.WriteStartObject();

            do
            {
                int row = 0;
                int dataSetCnt = 0;
                while (await reader.ReadAsync())
                {
                    if (row++ == 0)
                    {
                        string currDataSetName;
                        if (dataSetNames != null && dataSetNames.Length > dataSetCnt)
                        {
                            currDataSetName = dataSetNames[dataSetCnt];
                        }
                        else
                        {
                            currDataSetName = $"List{dataSetCnt}";
                        }
                        writer.WritePropertyName(currDataSetName);
                        writer.WriteStartArray();
                    }
                    writer.WriteStartObject();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        writer.WritePropertyName(reader.GetName(i));
                        if (reader.IsDBNull(i))
                        {
                            writer.WriteNullValue();
                        }
                        else
                        {
                            object columnValue = reader.GetValue(i);
                            JsonSerializer.Serialize(writer, columnValue, options);
                        }
                    }
                    writer.WriteEndObject();
                }
                writer.WriteEndArray();
                dataSetCnt++;
            } while (await reader.NextResultAsync());
            writer.WriteEndObject();


        }
    }
}
