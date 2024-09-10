namespace Puppy.Types;

public class DictDataEntryValuesState : IDataEntryValuesState
{
    private readonly Dictionary<string, object?> _values = new Dictionary<string, object?>();

    public int GetInt(string propName)
    {
        return (int) (_values[propName] ?? 0);
    }

    public void SetNullValue(string propName)
    {
        _values[propName] = null;
    }

    public void SetValueInt(string propName, int newValue)
    {
        _values[propName] = newValue;
    }

    public void SetValueString(string propName, string? newValue)
    {
        throw new NotImplementedException();
    }

    public void SetValueDecimal(string propName, decimal newValue)
    {
        throw new NotImplementedException();
    }

    public void SetValueDateTime(string propName, DateTime newValue)
    {
        throw new NotImplementedException();
    }

    public void SetValueDateTimeOffset(string propName, DateTimeOffset newValue)
    {
        throw new NotImplementedException();
    }

    public void SetValueTimeOnly(string propName, TimeOnly newValue)
    {
        throw new NotImplementedException();
    }

    public void SetValueInt(string propName, string? newValue)
    {
        if (string.IsNullOrEmpty(newValue))
        {
            _values[propName] = null;
        }
        else
        {
            _values[propName] = int.Parse(newValue);
        }
    }

    public void SetValueLong(string propName, string? newValue)
    {
        if (string.IsNullOrEmpty(newValue))
        {
            _values[propName] = null;
        }
        else
        {
            _values[propName] = int.Parse(newValue);
        }
    }
    
    public void SetValueDecimal(string propName, string? newValue)
    {
        if (string.IsNullOrEmpty(newValue))
        {
            _values[propName] = null;
        }
        else
        {
            _values[propName] = decimal.Parse(newValue);
        }
    }

    public void SetValueDateTime(string propName, string? newValue)
    {
        if (string.IsNullOrEmpty(newValue))
        {
            _values[propName] = null;
        }
        else
        {
            _values[propName] = DateTime.Parse(newValue);
        }
    }

    public void SetValueDateTimeOffset(string propName, string? newValue)
    {
        if (string.IsNullOrEmpty(newValue))
        {
            _values[propName] = null;
        }
        else
        {
            _values[propName] = DateTimeOffset.Parse(newValue);
        }
    }

    public void SetValueTimeOnly(string propName, string? newValue)
    {
        if (string.IsNullOrEmpty(newValue))
        {
            _values[propName] = null;
        }
        else
        {
            _values[propName] = TimeOnly.Parse(newValue);
        }
    }
}