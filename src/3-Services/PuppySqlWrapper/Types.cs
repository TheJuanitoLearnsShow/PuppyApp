using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using LanguageExt;
using LanguageExt.Common;

namespace PuppySqlWrapper;

public interface IPossibleValues
{
    
} 
public record LookupInfo(string ObjectForSearch, string SearchParameterName,
    string IdColumnName,string LabelColumnName, bool IsStoredProc, string ConnectionString) : IPossibleValues;
    
    
public record ValuesListFromDb(string ObjectForSearch, 
    string IdColumnName,string LabelColumnName, bool IsStoredProc, string ConnectionString) : IPossibleValues;
    
public record ValuesList(IReadOnlyList<ValueLabelPair> Values) : IPossibleValues;

public record ValueLabelPair(string Value, string Label)
{
}

public record PuppySqlType(
    string ParamName,
    bool Required,
    int Length,
    string ClrTypeName,
    int Decimals,
    string BaseSqlTypeName
)
{
    public string NonSqlParamName => ParamName[1..];
};

public record PuppySqlPossibleValuesType(
    string ParamName,
    bool Required,
    int Length,
    string ClrTypeName,
    int Decimals,
    string BaseSqlTypeName,
    IPossibleValues PossibleValues
) : PuppySqlType(ParamName, Required, Length, ClrTypeName, Decimals, BaseSqlTypeName);

public class IntValidationService
{
    public Either<IReadOnlyCollection<string>, Int64> Parse(PuppySqlType type, string stringValue)
    {
        // TODO check for specific .net type?
        if( System.Int64.TryParse(stringValue, out var value) )
        {
            return value;
        }
        else
        {
            return new string[] { "Must be a valid integer" };
        }
    }
    private static readonly Dictionary<string, SqlDbType> SqlBaseTypesTranslations = new()
    {
        { "bigint", SqlDbType.BigInt },
        { "decimal", SqlDbType.Decimal },
        { "int", SqlDbType.Int },
        { "money", SqlDbType.Money },
        { "numeric", SqlDbType.Decimal },
        { "smallint", SqlDbType.Int },
        { "smallmoney", SqlDbType.Decimal },
        { "tinyint", SqlDbType.Decimal },
        { "date", SqlDbType.Date },
        {
            "datetime",
            SqlDbType.DateTime
        },
        { "datetime2", 
            SqlDbType.DateTime2 },
        { "datetimeoffset ", SqlDbType.DateTimeOffset },
        { "smalldatetime", SqlDbType.DateTime },
        { "char", SqlDbType.Char },
        { "text", SqlDbType.Text },
        { "varchar", SqlDbType.VarChar },
        { "nchar", SqlDbType.NChar },
        { "ntext", SqlDbType.NText },
        { "nvarchar", SqlDbType.NVarChar },
        { "sysname", SqlDbType.VarChar },
        {"xml", SqlDbType.Xml}

    };
    public Either<IReadOnlyCollection<string>, SqlParameter> CreateSqlParam(PuppySqlType type, string stringValue)
    {
        return Parse(type, stringValue).Map(v =>
        {
            var newParameter = type.ClrTypeName == nameof(String) ? 
                new SqlParameter(type.ParamName,
                SqlBaseTypesTranslations[type.BaseSqlTypeName], type.Length) :
                new SqlParameter(type.ParamName,
                    SqlBaseTypesTranslations[type.BaseSqlTypeName]);
            newParameter.Value = v;
            return newParameter;
        });
    }
}