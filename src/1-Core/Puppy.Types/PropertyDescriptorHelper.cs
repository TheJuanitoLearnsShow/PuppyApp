namespace Puppy.Types;

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
    
    // public static DataEntryValidationResult SetValue<T>(string name, Func<string?, PropertyError[]> validate, 
    //     IDataEntryTypedValuesState stateOutput, T? input)
    // {
    //     var errors = validate(input);
    //     if (errors.Length == 0)
    //     {
    //         if (input == null)
    //         {
    //             stateOutput.SetNullValue(name);
    //         }
    //         else
    //         {
    //             stateOutput.SetValue(name, input);
    //         }
    //     }
    //     return new DataEntryValidationResult()
    //     {
    //         Errors = errors
    //     };
    // }
}