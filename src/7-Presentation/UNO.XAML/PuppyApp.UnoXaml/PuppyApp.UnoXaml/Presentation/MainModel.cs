using Puppy.Types;

namespace PuppyApp.UnoXaml.Presentation;

public partial record MainModel
{
    private INavigator _navigator;

    public MainModel(
        IStringLocalizer localizer,
        IOptions<AppConfig> appInfo,
        INavigator navigator)
    {
        _navigator = navigator;
        Title = "Main";
        Title += $" - {localizer["ApplicationName"]}";
        Title += $" - {appInfo?.Value?.Environment}";
        var props = new StringPropertyDescriptor("MiddleName", 3, true);
        Editors = (new IPrimitivePropertyDescriptor[] {
            
            new IntPropertyDescriptor("Age", 3, true ),
            new IntPropertyDescriptor("Grade", 2, false ),
            new StringPropertyDescriptor("MiddleName", 3, true ),
            new DateTimeOffsetPropertyDescriptor("EnrollmentDate", true )
            
        }).Select(p => new BindableCallParameterModel(p)).ToArray();
    }

    public string? Title { get; }

    public IState<string> Name => State<string>.Value(this, () => string.Empty);

    public async Task GoToSecond()
    {
        var name = await Name;
        await _navigator.NavigateViewModelAsync<SecondModel>(this, data: new Entity(name!));
    }
    public BindableCallParameterModel[] Editors {get;set;}
}
