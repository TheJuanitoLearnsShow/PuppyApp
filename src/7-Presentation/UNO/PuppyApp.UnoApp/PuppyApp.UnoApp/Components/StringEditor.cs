namespace PuppyApp.UnoApp.Components;

public class StringEditor
{
    public static UIElement Create(Func<BindableCallParameterModel> func)
    {
        return new StackPanel()
            .DataContext(func, ConfigureElement);
        
        void ConfigureElement(StackPanel p, BindableCallParameterModel vm)
        {
            p.Grid(row: 1)
                .HorizontalAlignment(HorizontalAlignment.Center)
                .VerticalAlignment(VerticalAlignment.Center)
                .Spacing(16)
                .Children(
                    new TextBlock()
                        .Text(x => x.Binding(() => 
                                vm.Label)),
                    new TextBox()
                        .Text(x => x.Binding(() => vm.EditValue).Mode(BindingMode.TwoWay))
                    
                );
        }
    }
}
