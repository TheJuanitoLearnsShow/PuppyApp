namespace Puppy.Types;

public class StringPropertyDescriptor : IPrimitivePropertyDescriptor
{
    public int MinLength { get; }
    public bool IsRequired { get; }
    public LabelValuePair[] AllowedValues { get; }

    public int MaxLen { get; }

    public StringPropertyDescriptor(string propertyName, int maxLength, bool isRequired)
    {
        Name = propertyName;
        IsRequired = isRequired;
        MaxLen = maxLength;
        AllowedValues = [];
    }
    public StringPropertyDescriptor(string propertyName, int maxLength, bool isRequired, LabelValuePair[] allowedValues)
    {
        Name = propertyName;
        IsRequired = isRequired;
        MaxLen = maxLength;
        AllowedValues = allowedValues;
    }
    public StringPropertyDescriptor(string propertyName, int minLength, int maxLength, bool isRequired, LabelValuePair[] allowedValues)
    {
        MinLength = minLength;
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
    
    public PropertyError[] SetValue(string? input, IDataEntryTypedValuesState stateOutput)
    {
        var errors = Validate(input);
        if (errors.Length == 0)
        {
            if (input == null)
            {
                stateOutput.SetNullValue(Name);
            }
            else
            {
                stateOutput.SetValue(Name, input);
            }
        }
        stateOutput.SetErrors(Name, errors);
        return errors;
    }
    public PropertyError[] Validate(string? inputText)
    {
        if (IsRequired && string.IsNullOrWhiteSpace(inputText))
        {
            return [PropertyError.IsRequired];
        }

        if (inputText?.Length < MinLength)
        {
            return [ PropertyError.NotLongEnough(MinLength) ];
        }
        if (inputText?.Length > MaxLen)
        {
            return [ PropertyError.ExceedsLength(MaxLen) ];
        }
        
        if (AllowedValues.Length > 0 && !AllowedValues.Any(s => 
                string.Equals(inputText, s.Value, StringComparison.OrdinalIgnoreCase)))
        {
            return [ PropertyError.InvalidOption ];
        }
        return [];
    }

    public override string ToString()
    {
        return $"{Name} String {MaxLen}";
    }
}