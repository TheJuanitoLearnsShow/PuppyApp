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
        return PropertyDescriptorHelper.Parse(Name, Validate, stateOutput.SetValueDateTimeOffset, input);
    }
    
    public PropertyError[] Validate(string? inputText)
    {
        if (IsRequired && inputText == null)
        {
            return [PropertyError.IsRequired];
        }
        var isValidParsedValue = DateTimeOffset.TryParse(inputText, out var newValue);
        if (isValidParsedValue)
        {
            if (newValue < MinValue)
            {
                return [PropertyError.LessThanDate(MinValue)];
            }

            if (newValue > MaxValue)
            {
                return [PropertyError.MoreThanDate(MaxValue)];
            }
        }
        return [];
    }

    public override string ToString()
    {
        return Name;
    }
}