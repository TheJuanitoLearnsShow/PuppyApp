namespace Puppy.Types;

public class DateTimeOffsetPropertyDescriptor : IPrimitivePropertyDescriptor
{
    private readonly bool _isRequired;
    private readonly DateTimeOffset _maxValue;
    private readonly DateTimeOffset _minValue;

    public DateTimeOffsetPropertyDescriptor(string propertyName, bool isRequired)
    {
        Name = propertyName;
        _isRequired = isRequired;
        _maxValue = DateTimeOffset.MaxValue;
        _minValue = DateTimeOffset.MinValue;
    }

    public string Name { get; }

    public DataEntryValidationResult Parse(DataEntryInput input, IDataEntryValuesState stateOutput)
    {
        var inputText = input.PrimitiveValue;
        var isValid = true;
        DateTimeOffset? valueForOutput = null;
        PropertyError[] errors = [];
        if (string.IsNullOrEmpty(inputText))
        {
            valueForOutput = null;
        }

        if (_isRequired && inputText == null)
        {
            isValid = false;
            errors = [ PropertyError.IsRequired ];
        }
        var isValidParsedValue = DateTimeOffset.TryParse(inputText, out var newValue);
        if (isValidParsedValue)
        {
            valueForOutput = newValue;
        }
        else
        {
            isValid = false;
            errors = [PropertyError.InvalidDateTimeValue()];
        }

        if (isValid)
        {
            stateOutput.SetValue(Name, valueForOutput);
        }
        return new DataEntryValidationResult()
        {
            Errors = errors
        };
    }

    public override string ToString()
    {
        return Name;
    }
}