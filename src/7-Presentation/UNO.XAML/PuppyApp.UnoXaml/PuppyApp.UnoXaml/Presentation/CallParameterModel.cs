using Humanizer;
using Puppy.Types;

namespace PuppyApp.UnoXaml.Presentation;

public partial record CallParameterModel
{
    public IPrimitivePropertyDescriptor PropertyDescriptor { get; }

    public CallParameterModel(IPrimitivePropertyDescriptor propertyDescriptor)
    {
        PropertyDescriptor = propertyDescriptor;
        Label = State<string>.Value(this, () => propertyDescriptor.Name.Humanize());
    }

    public IState<string> EditValue => State<string>.Value(this, () => string.Empty);
    public IState<string> Label
    {
        get;
    }

    public IFeed<string> Error => EditValue.Select(this.ValidateEditValue);

    private string ValidateEditValue(string editValue)
    {
        var errors = PropertyDescriptor.Validate(editValue);
        return string.Join(" | ", errors.Select(e => e.Description));
    }
}
