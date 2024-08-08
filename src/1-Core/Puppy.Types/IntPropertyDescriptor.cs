namespace Puppy.Types;

public class IntPropertyDescriptor : IPrimitivePropertyDescriptor
{
    private readonly bool _isRequired;
    private readonly int _maxValue;
    private readonly int _minValue;

    public IntPropertyDescriptor(string propertyName, int numDigits, bool isRequired)
    {
        Name = propertyName;
        _isRequired = isRequired;
        _maxValue = (int) Math.Pow(10, numDigits);
        _minValue = -_maxValue;
    }

    public string Name { get; }

    public DataEntryValidationResult Parse(DataEntryInput input, IDataEntryValuesState stateOutput)
    {
        var inputText = input.PrimitiveValue;
        var isValid = true;
        int? valueForOutput = 0;
        PropertyError[] errors = [];
        if (string.IsNullOrEmpty(inputText))
        {
            valueForOutput = null;
        }

        if (_isRequired && valueForOutput == null)
        {
            isValid = false;
            errors = [new PropertyError("Is required")];
        }
        var isNumeric = int.TryParse(inputText, out var newValue);
        if (isNumeric)
        {
            valueForOutput = newValue;
            if (newValue < _minValue)
            {
                isValid = false;
                errors = [new PropertyError($"Must not be less than {_minValue:N0}")];
            } else if (newValue > _maxValue)
            {
                isValid = false;
                errors = [new PropertyError($"Must not be greater than {_maxValue:N0}")];
            }
        }
        else
        {
            isValid = false;
            errors = [new PropertyError("Not a valid integer")];
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