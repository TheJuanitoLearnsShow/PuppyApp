namespace Puppy.Types;

public interface IPrimitiveProperty : IProperty
{
    public void Parse(string? inputText);
}