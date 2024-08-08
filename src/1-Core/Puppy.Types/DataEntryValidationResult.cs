namespace Puppy.Types;

/// <summary>
/// This class is the union state between a primitive errors (Errors), a collection errors (CollectionEntries)
/// and children properties errors (ChildrenEntries)
/// </summary>
public class DataEntryValidationResult
{
    public bool IsValid => Errors.Length == 0 && AreChildrenValid;
    public PropertyError[] Errors { get; set; } = [];
    
    public Dictionary<string, DataEntryValidationResult>? ChildrenEntries { get; set; }
    public bool AreChildrenValid => ChildrenEntries == null || ChildrenEntries.Values.All(c => c.IsValid);
    
    /// <summary>
    /// Each entry contains the errors for the respective entry in the input collection
    /// </summary>
    public DataEntryValidationResult[]? CollectionEntries { get; set; }
    public bool AreItemsValid => CollectionEntries == null || CollectionEntries.All(c => c.IsValid);
}