namespace Puppy.Types;

public class StringPropertyDescriptor : IPrimitivePropertyDescriptor
{
    private readonly bool _isRequired;
    private readonly int _maxLen;

    public StringPropertyDescriptor(string propertyName, int maxLength, bool isRequired)
    {
        Name = propertyName;
        _isRequired = isRequired;
        _maxLen = maxLength;
    }

    public string Name { get; }

    public DataEntryValidationResult Parse(DataEntryInput input, IDataEntryValuesState stateOutput)
    {
        var inputText = input.PrimitiveValue;
        var isValid = true;
        PropertyError[] errors = [];

        if (_isRequired && inputText == null)
        {
            isValid = false;
            errors = [PropertyError.IsRequired];
        }
        else if (inputText?.Length > _maxLen)
        {
            isValid = false;
            errors = [ PropertyError.ExceedsLength(_maxLen) ];
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