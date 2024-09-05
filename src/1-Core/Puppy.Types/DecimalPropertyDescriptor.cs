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
        return PropertyDescriptorHelper.Parse(Name, Validate, stateOutput.SetValueDecimal, input);
    }
    
    public PropertyError[] Validate(string? inputText)
    {
        if (IsRequired && string.IsNullOrWhiteSpace(inputText))
        {
            return [ PropertyError.IsRequired ];
        }
        var isNumeric = decimal.TryParse(inputText, out var newValue);
        if (isNumeric)
        {
            if (newValue < MinValue)
            {
                return [PropertyError.LessThanMinValue(MinValue)];
            }
            if (newValue > MaxValue)
            {
                return [PropertyError.MoreThanMaxValue(MaxValue)];
            }
        }
        else
        {
            return [PropertyError.InvalidDecimal];
        }
        return [];
    }

    public PropertyError[] Validate(decimal? newValue)
    {
        if (IsRequired && newValue == null)
        {
            return [PropertyError.IsRequired];
        }
        if (newValue < MinValue)
        {
            return [PropertyError.LessThanMinValue(MinValue)];
        }
        if (newValue > MaxValue)
        {
            return [PropertyError.MoreThanMaxValue(MaxValue)];
        }
        return [];
    }
    public override string ToString()
    {
        return Name;
    }
}