namespace Puppy.Types;

public class IntPropertyDescriptor : IPrimitivePropertyDescriptor
{
    public bool IsRequired { get; }

    public int MaxValue { get; }

    public int MinValue { get; }

    public IntPropertyDescriptor(string propertyName, int numDigits, bool isRequired)
    {
        Name = propertyName;
        IsRequired = isRequired;
        MaxValue = (int) Math.Pow(10, numDigits);
        MinValue = -MaxValue;
    }
    public IntPropertyDescriptor(string propertyName, int numDigits, bool isRequired, int minValue, int maxValue)
    {
        Name = propertyName;
        IsRequired = isRequired;
        MinValue = minValue;
        MaxValue = maxValue;
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

        if (IsRequired && inputText == null)
        {
            isValid = false;
            errors = [PropertyError.IsRequired];
        }
        var isNumeric = int.TryParse(inputText, out var newValue);
        if (isNumeric)
        {
            valueForOutput = newValue;
            if (newValue < MinValue)
            {
                isValid = false;
                errors = [PropertyError.LessThanMinValue(MinValue)];
            } 
            else if (newValue > MaxValue)
            {
                isValid = false;
                errors = [PropertyError.MoreThanMaxValue(MaxValue)];
            }
        }
        else
        {
            isValid = false;
            errors = [PropertyError.InvalidValue<int>()];
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