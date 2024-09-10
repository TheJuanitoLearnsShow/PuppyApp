namespace Puppy.Types;

public class LongPropertyDescriptor : IPrimitivePropertyDescriptor
{
    public bool IsRequired { get; }
    public long MaxValue { get; }
    public long MinValue { get; }

    public LongPropertyDescriptor()
    {
        Name = string.Empty;
        IsRequired = false;
        MaxValue = 0;
        MinValue = -MaxValue;
    }

    public LongPropertyDescriptor(string propertyName, int numDigits, bool isRequired)
    {
        Name = propertyName;
        IsRequired = isRequired;
        MaxValue = (long)Math.Pow(10, numDigits);
        MinValue = -MaxValue;
    }

    public LongPropertyDescriptor(string propertyName, int numDigits, bool isRequired, long minValue, long maxValue)
    {
        Name = propertyName;
        IsRequired = isRequired;
        MinValue = minValue;
        MaxValue = maxValue;
    }

    public string Name { get; }

    public DataEntryValidationResult Parse(DataEntryInput input, IDataEntryValuesState stateOutput)
    {
        return PropertyDescriptorHelper.Parse(Name, Validate, stateOutput.SetValueLong, input);
    }
    // public DataEntryValidationResult Parse(DataEntryInput input, IDataEntryTypedValuesState stateOutput)
    // {
    //     return PropertyDescriptorHelper.Parse(Name, Validate, stateOutput.SetValueLong, input);
    // }
    
    public PropertyError[] Validate(string? inputText)
    {
        if (IsRequired && string.IsNullOrWhiteSpace(inputText))
        {
            return new[] { PropertyError.IsRequired };
        }

        if (long.TryParse(inputText, out var newValue))
        {
            if (newValue < MinValue)
            {
                return new[] { PropertyError.LessThanMinValue(MinValue) };
            }

            if (newValue > MaxValue)
            {
                return new[] { PropertyError.MoreThanMaxValue(MaxValue) };
            }
        }
        else
        {
            return new[] { PropertyError.InvalidLong };
        }

        return Array.Empty<PropertyError>();
    }

    public PropertyError[] Validate(long? newValue)
    {
        if (IsRequired && newValue == null)
        {
            return new[] { PropertyError.IsRequired };
        }

        if (newValue < MinValue)
        {
            return new[] { PropertyError.LessThanMinValue(MinValue) };
        }

        if (newValue > MaxValue)
        {
            return new[] { PropertyError.MoreThanMaxValue(MaxValue) };
        }

        return Array.Empty<PropertyError>();
    }

    public override string ToString()
    {
        return Name;
    }
}