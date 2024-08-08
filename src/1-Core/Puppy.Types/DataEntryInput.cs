namespace Puppy.Types;

/// Simple class to make serialization easier
public class DataEntryInput
{
    public string? PrimitiveValue { get; set; }
    public Dictionary<string, DataEntryInput>? ChildrenEntries { get; set; }
}