namespace Puppy.Types;

public interface IPropertyDescriptor
{
    public string Name { get; }
    
    // TODO we might not need this Parse method anymore based on the experience from Blazor: the caller is the one owning the state
    //  so they can have the children props update their own values into the state that the caller creater. It will be up
    //  to the caller to build the tree, but it can do that by passing the IDataEntryTypedValuesState to the SetValue method
    //  of the properties it builds
    public DataEntryValidationResult Parse(DataEntryInput input, IDataEntryValuesState stateOutput); 
}