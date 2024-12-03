using Puppy.Types;

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
        if (item is PuppyApp.UnoXaml.Presentation.BindableCallParameterModel p) {
            
            var templateToUse = p.PropertyDescriptor switch
            {
                LongPropertyDescriptor ip => StringEditorTemplate,
                IntPropertyDescriptor ip => StringEditorTemplate,
                DecimalPropertyDescriptor ip => StringEditorTemplate,
                StringPropertyDescriptor sp => StringEditorTemplate,
                DateTimeOffsetPropertyDescriptor dtop => StringEditorTemplate,
                _ => StringEditorTemplate
            };
            return templateToUse!;
        }
        System.Diagnostics.Debug.WriteLine($"SelectTemplateCore with container called with item type: {item?.GetType()?.FullName}");
        return StringEditorTemplate;
    }
}
