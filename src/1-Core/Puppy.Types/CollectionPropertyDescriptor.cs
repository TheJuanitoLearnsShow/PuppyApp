namespace Puppy.Types;

public class CollectionPropertyDescriptor : IPropertyDescriptor
{
    private readonly string _propertyName;
    private readonly IEnumerable<ComplexPropertyDescriptor> _entries;
    private IEnumerable<PropertyError> _errors = [];

    public CollectionPropertyDescriptor(IEnumerable<ComplexPropertyDescriptor> entries, string propertyName)
    {
        _entries = entries;
        _propertyName = propertyName;
    }
    public string Name => _propertyName;
    
    public DataEntryValidationResult Parse(DataEntryInput input, IDataEntryValuesState stateOutput)
    {
        var results = _entries.Select(e => e.Parse(input, stateOutput)).ToArray(); // TODO what about TVP params in sql?

        return new DataEntryValidationResult()
        {
            CollectionEntries = results
        };
    }
    
}