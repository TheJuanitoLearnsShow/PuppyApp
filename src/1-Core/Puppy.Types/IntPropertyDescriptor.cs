using System.Diagnostics.CodeAnalysis;

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
        return PropertyDescriptorHelper.Parse(Name, Validate, stateOutput.SetValueInt, input);
    }

    public PropertyError[] Validate(string? inputText)
    {
        if (IsRequired && inputText == null)
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
            return [PropertyError.InvalidValue<int>()];
        }

        return [];
    }

    public override string ToString()
    {
        return Name;
    }
}