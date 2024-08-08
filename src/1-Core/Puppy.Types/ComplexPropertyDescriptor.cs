namespace Puppy.Types;

public class ComplexPropertyDescriptor : IPropertyDescriptor
{
    private readonly string _propertyName;
    private readonly IEnumerable<IPropertyDescriptor> _childProperties;

    public ComplexPropertyDescriptor(IEnumerable<IPropertyDescriptor> childProperties, string propertyName)
    {
        _childProperties = childProperties;
        _propertyName = propertyName;
    }
    
    public string Name => _propertyName;

    public IEnumerable<IPropertyDescriptor> ChildProperties => _childProperties;
    
    public DataEntryValidationResult Parse(DataEntryInput input, IDataEntryValuesState stateOutput)
    {
        var results = new Dictionary<string, DataEntryValidationResult>();
        foreach (var prop in _childProperties)
        {
            var inputValue = input.ChildrenEntries?[prop.Name];
            if (inputValue != null)
            {
                results[prop.Name] = prop.Parse(inputValue, stateOutput);
            }
        }
        return new DataEntryValidationResult()
        {
            ChildrenEntries = results
        };
    }
}