namespace Puppy.Types;

public class DateTimeOffsetPropertyDescriptor : IPrimitivePropertyDescriptor
{
    public bool IsRequired { get; }

    public DateTimeOffset MaxValue { get; }

    public DateTimeOffset MinValue { get; }

    public DateTimeOffsetPropertyDescriptor(string propertyName, bool isRequired)
    {
        Name = propertyName;
        IsRequired = isRequired;
        MaxValue = DateTimeOffset.MaxValue;
        MinValue = DateTimeOffset.MinValue;
    }
    public DateTimeOffsetPropertyDescriptor(string propertyName, bool isRequired, DateTimeOffset minValue, 
        DateTimeOffset maxValue)
    {
        Name = propertyName;
        IsRequired = isRequired;
        MaxValue = maxValue;
        MinValue = minValue;
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

        if (IsRequired && inputText == null)
        {
            isValid = false;
            errors = [ PropertyError.IsRequired ];
        }
        var isValidParsedValue = DateTimeOffset.TryParse(inputText, out var newValue);
        if (isValidParsedValue)
        {
            valueForOutput = newValue;
            if (newValue < MinValue)
            {
                isValid = false;
                errors = [PropertyError.LessThanDate(MinValue)];
            } 
            else if (newValue > MaxValue)
            {
                isValid = false;
                errors = [PropertyError.MoreThanDate(MaxValue)];
            }
        }
        else
        {
            isValid = false;
            errors = [ PropertyError.InvalidDateTimeValue ];
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