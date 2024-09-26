using Puppy.Types;

namespace PuppyApp.UnoApp.Presentation;

public partial record CallParameterModel
{
    private readonly StringPropertyDescriptor _propertyDescriptor;

    public CallParameterModel(StringPropertyDescriptor propertyDescriptor)
    {
        _propertyDescriptor = propertyDescriptor;
        Label = State<string>.Value(this, () => propertyDescriptor.Name);
    }

    public IState<string> EditValue => State<string>.Value(this, () => string.Empty);
    public IState<string> Label
    {
        get;
    }

    public IFeed<string> Error => EditValue.Select(this.ValidateEditValue);

    private string ValidateEditValue(string editValue)
    {
        var errors = _propertyDescriptor.Validate(editValue);
        return string.Join(" | ", errors.Select(e => e.Description));
    }
}
