namespace Puppy.Types;

public class DataEntryInput
{
    public string? PrimitiveValue { get; set; }
    public Dictionary<string, DataEntryInput>? ChildrenEntries { get; set; }
}