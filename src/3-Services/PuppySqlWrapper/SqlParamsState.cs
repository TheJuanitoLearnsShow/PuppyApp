using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Puppy.Types;

namespace PuppySqlWrapper;

public class SqlParamsState : IDataEntryTypedValuesState
{
    private readonly Dictionary<string, SqlParameter> _sqlParameters;
    private readonly Dictionary<string, PropertyError[]> _errors;

    public SqlParamsState(ComplexPropertyDescriptor paramDescriptor)
    {
        _errors = new Dictionary<string, PropertyError[]>(paramDescriptor.ChildProperties.Length());
        _sqlParameters = new Dictionary<string, SqlParameter>(paramDescriptor.ChildProperties.Length());
        foreach (var childProperty in paramDescriptor.ChildProperties)
        {
            var propName = childProperty.Name;
            var sqlParamName = '@' + propName;
            var newSqlParam = childProperty switch
            {
                LongPropertyDescriptor ip => new SqlParameter(sqlParamName, SqlDbType.BigInt),
                IntPropertyDescriptor ip => new SqlParameter(sqlParamName, SqlDbType.Int),
                DecimalPropertyDescriptor ip => new SqlParameter(sqlParamName, SqlDbType.Decimal),
                StringPropertyDescriptor sp => new SqlParameter(sqlParamName, SqlDbType.NVarChar, sp.MaxLen),
                DateTimeOffsetPropertyDescriptor dtop => new SqlParameter(sqlParamName, SqlDbType.DateTimeOffset),
                _ => null
            };
            if (newSqlParam != null)
            {
                _sqlParameters.Add(propName, newSqlParam);
            }
        }
    }
    
    public void SetNullValue(string propName)
    {
        _sqlParameters[propName].Value = DBNull.Value;
    }

    public void SetValue(string propName, int newValue)
    {
        _sqlParameters[propName].Value = newValue;
    }

    public void SetValue(string propName, decimal newValue)
    {
        _sqlParameters[propName].Value = newValue;
    }

    public void SetValue(string propName, DateTime newValue)
    {
        _sqlParameters[propName].Value = newValue;
    }

    public void SetValue(string propName, DateTimeOffset newValue)
    {
        _sqlParameters[propName].Value = newValue;
    }

    public void SetValue(string propName, TimeOnly newValue)
    {
        _sqlParameters[propName].Value = newValue;
    }

    public void SetValue(string propName, string newValue)
    {
        _sqlParameters[propName].Value = newValue;
    }

    public void SetErrors(string name, PropertyError[] errors)
    {
        _errors[name] = errors;
    }
    
    public bool HasErrors() => _errors.Any(kv => kv.Value.Length != 0);

    public async Task<IDataReader> ExecuteReaderAsync(string connStr, string spName)
    {
        await using var connection = new SqlConnection(connStr);
        SqlCommand command = new(spName, connection);
        var sqlParamToUse = _sqlParameters.Select(kv => kv.Value).ToArray();
        command.Parameters.Clear();
        command.Parameters.AddRange(sqlParamToUse);
        connection.Open();
        return await command.ExecuteReaderAsync();
    }
}