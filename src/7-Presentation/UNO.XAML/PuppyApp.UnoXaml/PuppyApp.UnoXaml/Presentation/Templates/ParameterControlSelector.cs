namespace PuppyApp.UnoXaml.Presentation.Templates;

public class ParameterControlSelector : DataTemplateSelector
{
    private DataTemplate? stringEditorTemplate;

    public DataTemplate? StringEditorTemplate
    {
        get => stringEditorTemplate;
        set => stringEditorTemplate = value;
    }

    protected override DataTemplate SelectTemplateCore(object item)
    {
        System.Diagnostics.Debug.WriteLine($"SelectTemplateCore called with item type: {item?.GetType()?.FullName}");
        return StringEditorTemplate!;
    }

    protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
    {
        System.Diagnostics.Debug.WriteLine($"SelectTemplateCore with container called with item type: {item?.GetType()?.FullName}");
        return SelectTemplateCore(item!);
    }
}
