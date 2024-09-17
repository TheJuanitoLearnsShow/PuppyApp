namespace Puppy.Types;

public class DateTimeOffsetPropertyDescriptor : IPrimitivePropertyDescriptor
{
    private readonly DateTimeOffset _maxValue;
    private readonly DateTimeOffset _minValue;
    public int? MinValueDaysOffset { get; }
    public bool IsRequired { get; }
    public int? MaxValueDaysOffset { get; }

    public DateTimeOffset MaxValue => MaxValueDaysOffset == null ? _maxValue : 
        GetTodayDateTimeOffset().AddDays(MaxValueDaysOffset.Value);

    private static DateTimeOffset GetTodayDateTimeOffset()
    {
        return new DateTimeOffset(DateTime.SpecifyKind(DateTime.Today, DateTimeKind.Local));
    }

    public DateTimeOffset MinValue => MinValueDaysOffset == null ? _minValue: 
        GetTodayDateTimeOffset().AddDays(MinValueDaysOffset.Value);

    public DateTimeOffsetPropertyDescriptor(string propertyName, bool isRequired)
    {
        Name = propertyName;
        IsRequired = isRequired;
        _maxValue = DateTimeOffset.MaxValue;
        _minValue = DateTimeOffset.MinValue;
    }
    public DateTimeOffsetPropertyDescriptor(string propertyName, bool isRequired, DateTimeOffset minValue, 
        DateTimeOffset maxValue, int? minValueDaysOffset = null, 
        int? maxValueDaysOffset = null)
    {
        Name = propertyName;
        IsRequired = isRequired;
        _maxValue = maxValue;
        _minValue = minValue;
        MinValueDaysOffset = minValueDaysOffset;
        MaxValueDaysOffset = maxValueDaysOffset;
    }

    public string Name { get; }

    public DataEntryValidationResult Parse(DataEntryInput input, IDataEntryValuesState stateOutput)
    {
        return PropertyDescriptorHelper.Parse(Name, Validate, stateOutput.SetValueDateTimeOffset, input);
    }
    
    public PropertyError[] SetValue(DateTimeOffset? input, IDataEntryTypedValuesState stateOutput)
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
        if (IsRequired && inputText == null)
        {
            return [PropertyError.IsRequired];
        }
        var isValidParsedValue = DateTimeOffset.TryParse(inputText, out var newValue);
        if (isValidParsedValue)
        {
            if (newValue < MinValue)
            {
                return [PropertyError.LessThanDate(MinValue)];
            }

            if (newValue > MaxValue)
            {
                return [PropertyError.MoreThanDate(MaxValue)];
            }
        }
        else
        {
            return new[] { PropertyError.InvalidDateTime };
        }
        return [];
    }
    public PropertyError[] Validate(DateTimeOffset? newValue)
    {
        if (IsRequired && newValue == null)
        {
            return [PropertyError.IsRequired];
        }
        if (newValue < MinValue)
        {
            return [PropertyError.LessThanDate(MinValue)];
        }

        if (newValue > MaxValue)
        {
            return [PropertyError.MoreThanDate(MaxValue)];
        }
        return [];
    }

    public override string ToString()
    {
        return $"{Name} DateTimeOffset";
    }
}