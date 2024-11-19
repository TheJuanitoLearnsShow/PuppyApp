namespace PuppyApp.UnoXaml.Presentation.Templates;

internal class ParameterControlSelector : DataTemplateSelector
{
    
    public DataTemplate StringEditorTemplate { get; set; }
    protected override DataTemplate SelectTemplateCore(object item)
    { 
        var itemTypeName = item.GetType().FullName;
        return StringEditorTemplate;
    }
    
}
