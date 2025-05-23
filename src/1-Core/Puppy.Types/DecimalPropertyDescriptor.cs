﻿namespace Puppy.Types;

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
        var maxValByDigits =
            decimal.Parse(new string('9', numDigits - decimalPlaces) + '.' + new string('9', decimalPlaces));
        MinValue = -maxValByDigits;
        MaxValue = maxValByDigits;
    }
    public DecimalPropertyDescriptor(string propertyName, int numDigits, int decimalPlaces, bool isRequired, 
        decimal? minValue, decimal? maxValue)
    {
        Name = propertyName;
        NumDigits = numDigits;
        IsRequired = isRequired;
        DecimalPlaces = decimalPlaces;
        var maxValByDigits =
            decimal.Parse(new string('9', numDigits - decimalPlaces) + '.' + new string('9', decimalPlaces));
        MinValue = minValue ?? -maxValByDigits;
        MaxValue = maxValue ?? maxValByDigits;
    }
    
    public string Name { get; }
    public int NumDigits { get; }

    public int DecimalPlaces { get; }

    public DataEntryValidationResult Parse(DataEntryInput input, IDataEntryValuesState stateOutput)
    {
        return PropertyDescriptorHelper.Parse(Name, Validate, stateOutput.SetValueDecimal, input);
    }
    
    public PropertyError[] SetValue(decimal? input, IDataEntryTypedValuesState stateOutput)
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
                stateOutput.SetValue(Name, input.Value);
            }
        }
        stateOutput.SetErrors(Name, errors);
        return errors;
    }

    public PropertyError[] Validate(string? inputText)
    {
        if (string.IsNullOrWhiteSpace(inputText))
        {
            return IsRequired ? [PropertyError.IsRequired] : [];
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
        return $"{Name} Decimal {NumDigits}, {DecimalPlaces}";
    }
}