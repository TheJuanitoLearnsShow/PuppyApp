namespace PuppySqlWrapper.Metadata;

public record ParamAllowedValue(string ParamName, string Value, string Label);
public record PuppySqlParamType(
    string ParamName,
    bool Required,
    int Length,
    string ClrTypeName,
    int NumDigits,
    int Decimals
)
{
    public string NonSqlParamName => ParamName[1..];
};
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