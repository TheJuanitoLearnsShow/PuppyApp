namespace Puppy.Types;

public interface IPrimitivePropertyDescriptor : IPropertyDescriptor
{
    PropertyError[] Validate(string? inputText);
}