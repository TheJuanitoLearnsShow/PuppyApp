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
        var inputText = input.PrimitiveValue;
        var isValid = true;
        PropertyError[] errors = [];

        if (IsRequired && inputText == null)
        {
            isValid = false;
            errors = [PropertyError.IsRequired];
        }
        else if (inputText?.Length > MaxLen)
        {
            isValid = false;
            errors = [ PropertyError.ExceedsLength(MaxLen) ];
        }
        else if (AllowedValues.Length > 0 && !AllowedValues.Any(s => string.Equals(inputText, s, StringComparison.OrdinalIgnoreCase)))
        {
            isValid = false;
            errors = [ PropertyError.InvalidOption ];
        }

        if (isValid)
        {
            stateOutput.SetValue(Name, inputText);
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