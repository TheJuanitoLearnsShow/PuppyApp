using NJsonSchema;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PuppySqlWrapper
{
    public class SqlInputAsJsonProcessor
    {
        private static string spParamsTypesQry = @"
                    select
                    name ParameterName,  
                    type_name(user_type_id) TypeName,  
                    type_name(system_type_id) BaseSqlTypeName,
                    max_length MaxLen,
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
                    ";

        private static Dictionary<string, string> sqlTranslations = new()
        {
            { "bigint", "integer" },
            { "decimal", "number" },
            { "int", "integer" },
            { "money", "number" },
            { "numeric", "number" },
            { "smallint", "integer" },
            { "smallmoney", "number" },
            { "tinyint", "number" },
            { "date", "string" },
            {
                "datetime",
                "string"
            },
            { "datetime2", "string" },
            { "smalldatetime", "string" },
            { "char", "string" },
            { "text", "string" },
            { "varchar", "string" },
            { "nchar", "string" },
            { "ntext", "string" },
            { "nvarchar", "string" },
            { "sysname", "string" },
            {"xml", "string"}

        };
        public async Task ValidateJson(string spname, string jsonPayload, string spOpenApiSchema)
        {
            var schema = await JsonSchema.FromJsonAsync(spOpenApiSchema);
            var errors = schema.Validate(jsonPayload);
            foreach (var error in errors)
                Console.WriteLine(error.Path + ": " + error.Kind);

        }

        public async Task<string> GetJsonSchemaForStoredProc(string connStr, string spname)
        {
            using var connection = new SqlConnection(connStr);
            SqlCommand command = new(spParamsTypesQry, connection);
           
                command.Parameters.AddWithValue("@spName", spname);
            connection.Open();

            using SqlDataReader reader = command.ExecuteReader();

            using var fs = new MemoryStream();
            using var jsonWriter = new Utf8JsonWriter(fs);
            await WriteDataSetAsJsonSchemaAsync(jsonWriter, reader, null, spname);
            reader.Close();
        }

        public static async Task WriteDataSetAsJsonSchemaAsync(Utf8JsonWriter writer, SqlDataReader reader,
                    JsonSerializerOptions options, string spName)
        {

            writer.WriteStartObject();
            //          "$id": "https://example.com/person.schema.json",
            //"$schema": "https://json-schema.org/draft/2020-12/schema",
            //"title": "Person",
            //"type": "object",
            AddPropertyAndValue(writer, "$id", "https://example.com/person.schema.json");
            AddPropertyAndValue(writer, "$schema", "https://json-schema.org/draft/2020-12/schema");
            AddPropertyAndValue(writer, "title", $"{spName}Request");
            AddPropertyAndValue(writer, "type", "object");

            writer.WritePropertyName("properties");
            writer.WriteStartObject();

            var requiredFields = new LinkedList<string>();
            while (await reader.ReadAsync())
            {
                var propName = reader["ParameterName"].ToString()[1..];
                writer.WritePropertyName(propName);
                writer.WriteStartObject();
                
                var translatedType =
                     sqlTranslations.TryGetValue(reader["BaseSqlTypeName"].ToString(), out string t) ? t : "string";
                AddPropertyAndValue(writer, "type", translatedType);
                AddPropertyAndValue(writer, "type", translatedType);
                writer.WriteEndObject();

                if (propName.StartsWith("Optional", StringComparison.OrdinalIgnoreCase))
                {
                    requiredFields.AddLast(propName);
                }
            }
            writer.WriteEndObject();

            writer.WritePropertyName("required");
            writer.WriteStartArray();\
            foreach(var f in requiredFields)
            {
                writer.WriteStringValue(f);
            }
            writer.WriteEndArray();

            writer.WriteEndObject();


        }

        private static void AddPropertyAndValue(Utf8JsonWriter writer, string propName, string propValue)
        {
            writer.WritePropertyName(propName);
            writer.WriteStringValue(propValue);
        }
    }
}
