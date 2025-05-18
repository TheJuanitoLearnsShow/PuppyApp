using Puppy.Types;
using PuppyApp.ReactiveViewModels;
using Xunit.Abstractions;

namespace PuppyApp.UI.Tests;

public class ViewModelTests
{
    private readonly ITestOutputHelper _testOutputHelper;

    public ViewModelTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public void CallParameterTest()
    {
        var ageProp = new IntPropertyDescriptor("Age", 3, true);
        var vm = new CallParameterViewModel(ageProp)
        {
            EditValue = "2A"
        };
        var error = vm.Error;
        _testOutputHelper.WriteLine(error);
        Assert.NotNull(error);
    }
}