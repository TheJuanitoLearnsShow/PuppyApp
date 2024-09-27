namespace PuppyApp.UnoXaml.Presentation;

public sealed partial class Shell : UserControl, IContentControlProvider
{
    public Shell()
    {
        this.InitializeComponent();
    }

    public ContentControl ContentControl => Splash;
}