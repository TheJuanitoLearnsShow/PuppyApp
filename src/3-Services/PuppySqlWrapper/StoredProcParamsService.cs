using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Puppy.Types;
using PuppySqlWrapper.Metadata;

namespace PuppySqlWrapper;



public class StoredProcParamsService
{
	private const string PuppyMetadataSpExists = @"EXISTS (SELECT 1 FROM sys.procedures WHERE Name = '[puppy].[spGetStoredProcedureMetadata]')";
	public async Task<ComplexPropertyDescriptor> GetParametersAsPropDescriptorsUsingPuppy(string connStr, string spname)
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
			var isOptional = propName.StartsWith("Optional", StringComparison.OrdinalIgnoreCase) ||
			                 (bool)reader["UdtIsNullable"];
			var isRequired = !isOptional;
			var isNumericType = reader["IsNumericType"] is bool && (bool)reader["IsNumericType"];
			var prec = (int) reader["Prec"];
			var scale = (int) reader["Scale"];
			var maxLen = (short) reader["MaxLen"];
			IPropertyDescriptor newType = translatedType switch
			{
				nameof(Int32) => new IntPropertyDescriptor(propName, isRequired),
				nameof(Int64) => new LongPropertyDescriptor(propName, isRequired),
				nameof(Decimal) => new DecimalPropertyDescriptor(propName, prec, scale, isRequired),
				nameof(String) => new StringPropertyDescriptor(propName, maxLen, isRequired),
				nameof(DateTimeOffset) => new DateTimeOffsetPropertyDescriptor(propName, isRequired),
				nameof(DateTime) => new DateTimeOffsetPropertyDescriptor(propName, isRequired),
				_ => new StringPropertyDescriptor(propName, maxLen, isRequired)
			};
			parametersTypes.Add(newType);
		}
		await reader.CloseAsync();
		return new ComplexPropertyDescriptor(parametersTypes, spname);
	}
	
	public async Task<ComplexPropertyDescriptor> GetParametersAsPropDescriptorsNonPuppy(string connStr, string spname)
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
			var isOptional = propName.StartsWith("Optional", StringComparison.OrdinalIgnoreCase) ||
			                 (bool)reader["UdtIsNullable"];
			var isRequired = !isOptional;
			var isNumericType = reader["IsNumericType"] is bool && (bool)reader["IsNumericType"];
			var prec = (int) reader["Prec"];
			var scale = (int) reader["Scale"];
			var maxLen = (short) reader["MaxLen"];
			IPropertyDescriptor newType = translatedType switch
			{
				nameof(Int32) => new IntPropertyDescriptor(propName, isRequired),
				nameof(Int64) => new LongPropertyDescriptor(propName, isRequired),
				nameof(Decimal) => new DecimalPropertyDescriptor(propName, prec, scale, isRequired),
				nameof(String) => new StringPropertyDescriptor(propName, maxLen, isRequired),
				nameof(DateTimeOffset) => new DateTimeOffsetPropertyDescriptor(propName, isRequired),
				nameof(DateTime) => new DateTimeOffsetPropertyDescriptor(propName, isRequired),
				_ => new StringPropertyDescriptor(propName, maxLen, isRequired)
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
		}
		await reader.CloseAsync();
		return parametersTypes;
	}
    private static string spParamsTypesQry = @"
                    select
					    p.name ParameterName,
					    type_name(p.user_type_id) TypeName,
					    type_name(p.system_type_id) BaseSqlTypeName,
					    p.max_length MaxLen,
					    case when type_name(p.system_type_id) = 'uniqueidentifier'
					             then p.precision
					         else OdbcPrec(p.system_type_id, p.max_length, p.precision) end Prec,
					    coalesce(OdbcScale(p.system_type_id, p.scale), -1) Scale,
					    case when OdbcScale(p.system_type_id, p.scale) is not null then cast(1 as bit)  else cast(0 as bit) end IsNumericType,
					    p.parameter_id ParamOrder,
					    case when t.is_user_defined = 1 and t.is_nullable = 1 then cast(1 as bit) else cast(0 as bit) end UdtIsNullable
					    
					from sys.parameters p
					left join sys.types t
						on p.user_type_id = t.user_type_id
					where object_id = object_id(@spName)
					order by p.parameter_id
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