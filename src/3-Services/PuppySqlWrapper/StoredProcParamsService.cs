using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Puppy.Types;

namespace PuppySqlWrapper;

public class StoredProcParamsService
{
	public async Task<ComplexPropertyDescriptor> GetParametersAsPropDescriptors(string connStr, string spname)
	{
		var parametersTypes = new List<IPropertyDescriptor>();
		await using var connection = new SqlConnection(connStr);
		SqlCommand command = new(spParamsTypesQry, connection);
           
		command.Parameters.AddWithValue("@spName", spname);
		connection.Open();
		var paramCount = 0;
		await using var reader = await command.ExecuteReaderAsync();
		while (await reader.ReadAsync())
		{
			paramCount++;
			var propName = reader["ParameterName"].ToString()?[1..] ?? $"param{paramCount}";
			
			var translatedType =
				SqlTranslations.TryGetValue(reader["BaseSqlTypeName"].ToString() ?? string.Empty, out var t) ? t : "string";
			var isRequired = !propName.StartsWith("Optional", StringComparison.OrdinalIgnoreCase);
			var isNumericType = reader["IsNumericType"] is bool && (bool)reader["IsNumericType"];
			var prec = (int) reader["Prec"];
			var scale = (int) reader["Scale"];
			var maxLen = (short) reader["MaxLen"];
			IPropertyDescriptor newType = translatedType switch
			{
				nameof(Int32) => new IntPropertyDescriptor(propName, prec, isRequired),
				nameof(Int64) => new LongPropertyDescriptor(propName, prec, isRequired),
				nameof(Decimal) => new DecimalPropertyDescriptor(propName, prec, scale, isRequired),
				nameof(String) => new StringPropertyDescriptor(propName, maxLen, isRequired),
				nameof(DateTimeOffset) => new DateTimeOffsetPropertyDescriptor(propName, isRequired),
			};
			parametersTypes.Add(newType);
		}
		await reader.CloseAsync();
		return new ComplexPropertyDescriptor(parametersTypes, spname);
	}
	public async Task<IReadOnlyCollection<PuppySqlType>> GetParametersTypes(string connStr, string spname)
	{
		var parametersTypes = new List<PuppySqlType>();
		await using var connection = new SqlConnection(connStr);
		SqlCommand command = new(spParamsTypesQry, connection);
           
		command.Parameters.AddWithValue("@spName", spname);
		connection.Open();

		await using var reader = await command.ExecuteReaderAsync();
		while (await reader.ReadAsync())
		{
			var propName = reader["ParameterName"].ToString()?[1..];
			
			var translatedType =
				SqlTranslations.TryGetValue(reader["BaseSqlTypeName"].ToString() ?? string.Empty, out var t) ? t : "string";
			var required = propName != null && !propName.StartsWith("Optional", StringComparison.OrdinalIgnoreCase);
			var isNumericType = reader["IsNumericType"] is bool && (bool)reader["IsNumericType"];
			var prec = (int) reader["Prec"];
			var maxLen = (short) reader["MaxLen"];
			var newType = new PuppySqlType(propName, required, isNumericType ? prec : maxLen, translatedType,
				(int)reader["Scale"], 
				reader["BaseSqlTypeName"].ToString() ?? string.Empty);
			parametersTypes.Add(newType);
			// if p.IsNumericType then 
			// {
			// 	Required = true
			// 	Length = p.Prec
			// 	Nature = p.BaseSqlTypeName |> Puppy.SqlMapper.SqlTypeTranslator.ToNature
			// 		Decimals = p.Scale
			// 	Ranges = getRangeFor p.TypeName
			// 		LookupInfo = lkInfo
			// }
			// else 
			// {
			// 	Required = true
			// 	Length = p.MaxLen
			// 	Nature = p.BaseSqlTypeName |> Puppy.SqlMapper.SqlTypeTranslator.ToNature
			// 		Decimals = 0
			// 	Ranges = getRangeFor p.TypeName
			// 		LookupInfo = lkInfo
			// }
		}
		await reader.CloseAsync();
		return parametersTypes;
	}
    private static string spParamsTypesQry = @"
                    select
                    name ParameterName,  
                    type_name(user_type_id) TypeName,  
                    type_name(system_type_id) BaseSqlTypeName,
                    max_length MaxLen,
                    case when type_name(system_type_id) = 'uniqueidentifier' 
                                then precision  
                                else OdbcPrec(system_type_id, max_length, precision) end Prec,
                    coalesce(OdbcScale(system_type_id, scale), -1) Scale, 
	                case when OdbcScale(system_type_id, scale) is not null then cast(1 as bit)  else cast(0 as bit) end IsNumericType,
                    parameter_id ParamOrder,  
                    convert(sysname,
                                    case when system_type_id in (35, 99, 167, 175, 231, 239)
                                    then ServerProperty('collation') end) Collation

                    from sys.parameters where object_id = object_id(@spName)
                    "; // TODO get check constraint of column and try very basic parsing
    private static readonly Dictionary<string, string> SqlTranslations = new()
    {
	    { "bigint", nameof(Int64) },
	    { "decimal", nameof(Decimal) },
	    { "int", nameof(Int32) },
	    { "money", nameof(Decimal) },
	    { "numeric", nameof(Decimal) },
	    { "smallint", nameof(Int32) },
	    { "smallmoney", nameof(Decimal) },
	    { "tinyint", nameof(Int32) },
	    { "date", 
		    nameof(DateTime)},
	    {
		    "datetime",
		    nameof(DateTime)
	    },
	    { "datetime2", nameof(DateTime) },
	    { "datetimeoffset", nameof(DateTimeOffset) },
	    { "datetimeoffset2", nameof(DateTimeOffset) },
	    { "smalldatetime", nameof(DateTime) },
	    { "char", nameof(String) },
	    { "text", nameof(String) },
	    { "varchar", nameof(String) },
	    { "nchar", nameof(String) },
	    { "ntext", nameof(String) },
	    { "nvarchar", nameof(String) },
	    { "sysname", nameof(String) },
	    {"xml", nameof(String)}

    };
    
}