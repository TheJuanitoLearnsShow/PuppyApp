namespace PuppyApp.UnoApp.Components;

public class IntEditor
{
    public static UIElement Create()
    {
        return new StackPanel()
            .Grid(row: 1)
            .HorizontalAlignment(HorizontalAlignment.Center)
            .VerticalAlignment(VerticalAlignment.Center)
            .Spacing(16)
            .Children(
                new TextBox()
                    .Text(x => x.Binding(() => "Age").Mode(BindingMode.TwoWay))
                    .PlaceholderText("Enter your age:")
        );
    }
}
