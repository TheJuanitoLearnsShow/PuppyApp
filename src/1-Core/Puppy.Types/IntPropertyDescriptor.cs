using System.Diagnostics.CodeAnalysis;

namespace Puppy.Types;

public class IntPropertyDescriptor : IPrimitivePropertyDescriptor
{
    public bool IsRequired { get; }

    public int MaxValue { get; }

    public int MinValue { get; }

    public IntPropertyDescriptor()
    {
        Name = string.Empty;
        IsRequired = false;
        MaxValue = 0;
        MinValue = -MaxValue;
    }
    public IntPropertyDescriptor(string propertyName, bool isRequired)
    {
        Name = propertyName;
        IsRequired = isRequired;
        MaxValue = int.MaxValue;
        MinValue = int.MinValue;
    }
    public IntPropertyDescriptor(string propertyName, int numDigits, bool isRequired)
    {
        Name = propertyName;
        IsRequired = isRequired;
        MaxValue = (int) Math.Pow(10, numDigits);
        MinValue = -MaxValue;
    }
    public IntPropertyDescriptor(string propertyName, bool isRequired, int minValue, int maxValue)
    {
        Name = propertyName;
        IsRequired = isRequired;
        MinValue = minValue;
        MaxValue = maxValue;
    }

    public string Name { get; }

    public DataEntryValidationResult Parse(DataEntryInput input, IDataEntryValuesState stateOutput)
    {
        return PropertyDescriptorHelper.Parse(Name, Validate, stateOutput.SetValueInt, input);
    }

    public PropertyError[] Validate(string? inputText)
    {
        if (IsRequired && string.IsNullOrWhiteSpace(inputText))
        {
            return [PropertyError.IsRequired];
        }
        var isNumeric = int.TryParse(inputText, out var newValue);
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
            return [PropertyError.InvalidInt];
        }

        return [];
    }

    public PropertyError[] Validate(int? newValue)
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
        return $"{Name} Int {MaxValue}";
    }
}