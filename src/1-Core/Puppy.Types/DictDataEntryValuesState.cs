namespace Puppy.Types;

public class DictDataEntryValuesState : IDataEntryValuesState
{
    private readonly Dictionary<string, object> _values = new Dictionary<string, object>();

    public int GetInt(string propName)
    {
        return (int) _values[propName];
    }
    public void SetValue(string propName, int newValue)
    {
        _values[propName] = newValue;
    }

    public void SetValue(string propName, string newValue)
    {
        throw new NotImplementedException();
    }

    public void SetValue(string propName, decimal newValue)
    {
        throw new NotImplementedException();
    }

    public void SetValue(string propName, DateTime newValue)
    {
        throw new NotImplementedException();
    }

    public void SetValue(string propName, DateTimeOffset newValue)
    {
        throw new NotImplementedException();
    }

    public void SetValue(string propName, TimeOnly newValue)
    {
        throw new NotImplementedException();
    }

    public void SetValue(string propName, int? newValue)
    {
        _values[propName] = newValue;
    }

    public void SetValue(string propName, decimal? newValue)
    {
        throw new NotImplementedException();
    }

    public void SetValue(string propName, DateTime? newValue)
    {
        throw new NotImplementedException();
    }

    public void SetValue(string propName, DateTimeOffset? newValue)
    {
        throw new NotImplementedException();
    }

    public void SetValue(string propName, TimeOnly? newValue)
    {
        throw new NotImplementedException();
    }
}