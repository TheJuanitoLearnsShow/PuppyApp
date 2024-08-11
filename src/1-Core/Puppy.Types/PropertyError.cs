using System.Numerics;

namespace Puppy.Types;

public enum ValidationErrorCode
{
    GenericValidation,
    IsRequired,
    LessThanMinValue,
    MoreThanMaxValue,
    InvalidValue,
    ExceedsLength,
    InvalidOption
}
public record PropertyError(string Description, ValidationErrorCode Code = ValidationErrorCode.GenericValidation)
{
    public static readonly PropertyError IsRequired = new ("Is required", ValidationErrorCode.IsRequired);
    public static PropertyError LessThanMinValue<T>(T minValue) where T:INumber<T> => new ($"Must not be less than {minValue:N0}", ValidationErrorCode.LessThanMinValue);
    public static PropertyError LessThanDate(DateTimeOffset minValue) => new ($"Must not be earlier than {minValue:yyyy-MM-dd hh:mm:ss t z}", ValidationErrorCode.LessThanMinValue);
    public static PropertyError MoreThanDate(DateTimeOffset maxValue) => new ($"Must not be later than {maxValue:yyyy-MM-dd hh:mm:ss t z}", ValidationErrorCode.MoreThanMaxValue);

    public static PropertyError MoreThanMaxValue<T>(T maxValue) where T:INumber<T> => new ($"Must not be greater than {maxValue:N0}", ValidationErrorCode.MoreThanMaxValue);
    public static PropertyError InvalidValue<T>() => new ($"Not a valid {typeof(T).Name}", ValidationErrorCode.InvalidValue);
    public static readonly PropertyError InvalidDateTimeValue = new ($"Not a valid date and time format", ValidationErrorCode.InvalidValue);
    
    public static readonly PropertyError InvalidOption = new ($"Not a valid value from the possible options", ValidationErrorCode.InvalidOption);
    public static PropertyError ExceedsLength(int maxLen) => new ($"Length cannot be more than {maxLen}", ValidationErrorCode.ExceedsLength);
}