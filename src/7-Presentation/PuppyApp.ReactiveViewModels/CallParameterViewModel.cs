using System.Reactive.Linq;
using Puppy.Types;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using System.Reactive.Concurrency;

namespace PuppyApp.ReactiveViewModels;

public partial class CallParameterModel : ReactiveObject
{
    public IPrimitivePropertyDescriptor PropertyDescriptor { get; }

    public CallParameterModel(IPrimitivePropertyDescriptor propertyDescriptor)
    {
        PropertyDescriptor = propertyDescriptor;
        
        _errorHelper = this.WhenAnyValue(x => x.EditValue) 
            .Throttle(TimeSpan.FromMilliseconds(800), RxApp.TaskpoolScheduler)
            .DistinctUntilChanged()
            .ToProperty(this, x => ValidateEditValue(x.EditValue));;
    }
    
    [Reactive]
    private string _editValue = string.Empty;
    [Reactive]
    private string _label = string.Empty;
    
    [ObservableAsProperty]
    private string _error = string.Empty;
    
    private string ValidateEditValue(string editValue)
    {
        var errors = PropertyDescriptor.Validate(editValue);
        return string.Join(" | ", errors.Select(e => e.Description));
    }
}
