namespace Puppy.Types;

public class StringPropertyDescriptor : IPrimitivePropertyDescriptor
{
    public bool IsRequired { get; }
    public string[] AllowedValues { get; }

    public int MaxLen { get; }

    public StringPropertyDescriptor(string propertyName, int maxLength, bool isRequired)
    {
        Name = propertyName;
        IsRequired = isRequired;
        MaxLen = maxLength;
    }
    public StringPropertyDescriptor(string propertyName, int maxLength, bool isRequired, string[] allowedValues)
    {
        Name = propertyName;
        IsRequired = isRequired;
        MaxLen = maxLength;
        AllowedValues = allowedValues;
    }

    public string Name { get; }
    
    
    public DataEntryValidationResult Parse(DataEntryInput input, IDataEntryValuesState stateOutput)
    {
        return PropertyDescriptorHelper.Parse(Name, Validate, stateOutput.SetValueString, input);
    }
    
    public PropertyError[] Validate(string? inputText)
    {
        if (IsRequired && inputText == null)
        {
            return [PropertyError.IsRequired];
        }

        if (inputText?.Length > MaxLen)
        {
            return [ PropertyError.ExceedsLength(MaxLen) ];
        }

        if (AllowedValues.Length > 0 && !AllowedValues.Any(s => string.Equals(inputText, s, StringComparison.OrdinalIgnoreCase)))
        {
            return [ PropertyError.InvalidOption ];
        }
        return [];
    }

    public override string ToString()
    {
        return Name;
    }
}