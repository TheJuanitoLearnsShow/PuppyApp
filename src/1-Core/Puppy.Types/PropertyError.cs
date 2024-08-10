using System.Numerics;

namespace Puppy.Types;

public enum ValidationErrorCode
{
    GenericValidation,
    IsRequired,
    LessThanMinValue,
    MoreThanMaxValue,
    InvalidValue,
    ExceedsLength
}
public record PropertyError(string Description, ValidationErrorCode Code = ValidationErrorCode.GenericValidation)
{
    public static readonly PropertyError IsRequired = new ("Is required", ValidationErrorCode.IsRequired);
    public static PropertyError LessThanMinValue<T>(T minValue) where T:INumber<T> => new ($"Must not be less than {minValue:N0}", ValidationErrorCode.LessThanMinValue);
    
    public static PropertyError MoreThanMaxValue<T>(T maxValue) where T:INumber<T> => new ($"Must not be greater than {maxValue:N0}", ValidationErrorCode.MoreThanMaxValue);
    public static PropertyError InvalidValue<T>() => new ($"Not a valid {typeof(T).Name}", ValidationErrorCode.InvalidValue);
    public static PropertyError ExceedsLength(int maxLen) => new ($"Length cannot be more than {maxLen}", ValidationErrorCode.ExceedsLength);
}