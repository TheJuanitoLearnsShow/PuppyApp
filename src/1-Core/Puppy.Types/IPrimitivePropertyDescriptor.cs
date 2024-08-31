namespace Puppy.Types;

public interface IPrimitivePropertyDescriptor : IPropertyDescriptor
{
    PropertyError[] Validate(string? inputText);
}

public static class PropertyDescriptorHelper
{
    public static DataEntryValidationResult Parse(string name, Func<string?, PropertyError[]> validate, 
        Action<string, string?> setValue,
        DataEntryInput input)
    {
        var inputText = input.PrimitiveValue;
        var errors = validate(inputText);
        var isValid = errors.Length == 0;

        if (!isValid)
            return new DataEntryValidationResult()
            {
                Errors = errors
            };
       
        setValue(name, inputText);
        return new DataEntryValidationResult()
        {
            Errors = errors
        };
    }
}