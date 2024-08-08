namespace Puppy.Types;

public interface IDataEntryValuesState
{
    public void SetValue(string propName, int newValue);
    public void SetValue(string propName, string newValue);
    public void SetValue(string propName, decimal newValue);
    public void SetValue(string propName, DateTime newValue);
    public void SetValue(string propName, DateTimeOffset newValue);
    public void SetValue(string propName, TimeOnly newValue);
    
    public void SetValue(string propName, int? newValue);
    public void SetValue(string propName, decimal? newValue);
    public void SetValue(string propName, DateTime? newValue);
    public void SetValue(string propName, DateTimeOffset? newValue);
    public void SetValue(string propName, TimeOnly? newValue);
}