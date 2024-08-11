namespace Puppy.Types;

public class DecimalPropertyDescriptor : IPrimitivePropertyDescriptor
{
    public bool IsRequired { get; }

    public decimal MaxValue { get; }

    public decimal MinValue { get; }

    public DecimalPropertyDescriptor(string propertyName, int numDigits, int decimalPlaces, bool isRequired)
    {
        Name = propertyName;
        NumDigits = numDigits;
        IsRequired = isRequired;
        DecimalPlaces = decimalPlaces;
        MaxValue = (decimal) Math.Pow(10, numDigits);
        MinValue = -MaxValue;
    }
    public DecimalPropertyDescriptor(string propertyName, int numDigits, int decimalPlaces, bool isRequired, 
        decimal minValue, decimal maxValue)
    {
        Name = propertyName;
        NumDigits = numDigits;
        IsRequired = isRequired;
        DecimalPlaces = decimalPlaces;
        MaxValue = maxValue;
        MinValue = minValue;
    }
    
    public string Name { get; }
    public int NumDigits { get; }

    public int DecimalPlaces { get; }

    public DataEntryValidationResult Parse(DataEntryInput input, IDataEntryValuesState stateOutput)
    {
        var inputText = input.PrimitiveValue;
        var isValid = true;
        decimal? valueForOutput = 0;
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
        var isNumeric = decimal.TryParse(inputText, out var newValue);
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
            errors = [PropertyError.InvalidValue<decimal>()];
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