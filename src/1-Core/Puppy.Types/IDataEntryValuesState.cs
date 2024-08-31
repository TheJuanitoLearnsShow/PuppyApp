namespace Puppy.Types;

public interface IDataEntryValuesState
{
    public void SetNullValue(string propName);
    public void SetValueInt(string propName, int newValue);
    public void SetValueString(string propName, string? newValue);
    public void SetValueDecimal(string propName, decimal newValue);
    public void SetValueDateTime(string propName, DateTime newValue);
    public void SetValueDateTimeOffset(string propName, DateTimeOffset newValue);
    public void SetValueTimeOnly(string propName, TimeOnly newValue);
    public void SetValueInt(string propName, string? newValue);
    public void SetValueDecimal(string propName, string? newValue);
    public void SetValueDateTime(string propName, string? newValue);
    public void SetValueDateTimeOffset(string propName, string? newValue);
    public void SetValueTimeOnly(string propName, string? newValue);
}