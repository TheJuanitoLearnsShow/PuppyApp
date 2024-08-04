namespace Puppy.Types;

public interface IProperty
{
    public string Name { get; }
    public bool IsValid { get; }
    public IEnumerable<PropertyError> Errors { get; }
    public void Parse(DataEntryInput input);
}