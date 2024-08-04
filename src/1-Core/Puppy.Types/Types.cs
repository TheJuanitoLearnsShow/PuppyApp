namespace Puppy.Types;

public class IntProperty : IPrimitiveProperty
{
    private readonly string _propertyName;
    private readonly int _numDigits;
    private readonly bool _isRequired;
    private readonly int _maxValue;
    private readonly int _minValue;
    private bool _isValid;
    private int? _value;
    private IEnumerable<PropertyError> _errors = [];

    public IntProperty(string propertyName, int numDigits, bool isRequired)
    {
        _propertyName = propertyName;
        _numDigits = numDigits;
        _isRequired = isRequired;
        _maxValue = 10 ^ numDigits;
        _minValue = -_maxValue;
        _isValid = true;
        _value = 0;
    }

    public string Name => _propertyName;
    public bool IsValid => _isValid;
    public IEnumerable<PropertyError> Errors => _errors;

    public void Parse(string? inputText)
    {
        if (string.IsNullOrEmpty(inputText))
        {
            _value = null;
        }

        if (_isRequired && _value == null)
        {
            _isValid = false;
            _errors = [new PropertyError("Is required")];
        }
        var isNumeric = int.TryParse(inputText, out var newValue);
        if (!isNumeric)
        {
            _isValid = false;
            _errors = [new PropertyError("Not a valid integer")];
        }
        _value = newValue;
        if (newValue < _minValue)
        {
            _isValid = false;
            _errors = [new PropertyError($"Must not be less than {_minValue:N0}")];
        }
        if (newValue > _maxValue)
        {
            _isValid = false;
            
            _errors = [new PropertyError($"Must not be greater than {_minValue:N0}")];
        }

        _isValid = true;
    }

    public void Parse(DataEntryInput input)
    {
        Parse(input.PrimitiveValue);
    }

    public override string ToString()
    {
        return _value?.ToString() ?? string.Empty;
    }
}

public class ComplexProperty : IProperty
{
    private readonly string _propertyName;
    private readonly IEnumerable<IProperty> _childProperties;
    private IEnumerable<PropertyError> _errors = [];

    public ComplexProperty(IEnumerable<IProperty> childProperties, string propertyName)
    {
        _childProperties = childProperties;
        _propertyName = propertyName;
    }
    
    public string Name => _propertyName;
    public bool IsValid => ChildProperties.All(p => p.IsValid);
    public IEnumerable<PropertyError> Errors => [];

    public IEnumerable<IProperty> ChildProperties => _childProperties;
    public void Parse(DataEntryInput input)
    {
        foreach (var prop in _childProperties)
        {
            var inputValue = input.ChildrenEntries?[prop.Name];
            if (inputValue != null)
            {
                prop.Parse(inputValue);
            }
        }
    }
}

public class CollectionProperty : IProperty
{
    private readonly string _propertyName;
    private readonly IEnumerable<ComplexProperty> _entries;
    private IEnumerable<PropertyError> _errors = [];

    public CollectionProperty(IEnumerable<ComplexProperty> entries, string propertyName)
    {
        _entries = entries;
        _propertyName = propertyName;
    }
    public string Name => _propertyName;
    public bool IsValid => _entries.All(p => p.IsValid);
    public IEnumerable<PropertyError> Errors => [];
    public void Parse(DataEntryInput input)
    {
        foreach (var prop in _entries)
        {
            var inputValue = input.ChildrenEntries?[prop.Name];
            if (inputValue != null)
            {
                prop.Parse(inputValue);
            }
        }
    }
}

public class DataEntryLoader
{
    private readonly ComplexProperty _rootForm;

    public DataEntryLoader(ComplexProperty rootForm)
    {
        _rootForm = rootForm;
    }

    public void ApplyValues(DataEntryInput inputRoot)
    {
        _rootForm.Parse(inputRoot);
    }
}