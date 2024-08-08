namespace Puppy.Types;

public interface IPropertyDescriptor
{
    public string Name { get; }
    public DataEntryValidationResult Parse(DataEntryInput input, IDataEntryValuesState stateOutput);
}