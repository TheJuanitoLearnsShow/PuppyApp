using Puppy.Types;

namespace PuppySqlWrapper.Metadata;

public record AllowedValue(string ParamName, string Value, string Label)
{
    public LabelValuePair ToLabelValuePair() => new LabelValuePair(Label, Value);
}
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