using System.Reactive.Linq;
using Puppy.Types;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using System.Reactive.Concurrency;

namespace PuppyApp.ReactiveViewModels;

public partial class CallParameterViewModel : ReactiveObject
{
    public IPrimitivePropertyDescriptor PropertyDescriptor { get; }

    public CallParameterViewModel(IPrimitivePropertyDescriptor propertyDescriptor)
    {
        PropertyDescriptor = propertyDescriptor;

        Label = propertyDescriptor.Name;
        // _errorHelper = this.WhenAnyValue(v => v.ObservableForProperty(x => x.EditValue) 
        //     .Throttle(TimeSpan.FromMilliseconds(800), RxApp.TaskpoolScheduler)
        //     .Select(x => x.Value)
        //     .DistinctUntilChanged()
        //     )
        //     .ToProperty(this, x => ValidateEditValue(x.EditValue));;
        
        _errorHelper = this.WhenAnyValue(x => x.EditValue) 
                .Throttle(TimeSpan.FromMilliseconds(800), RxApp.MainThreadScheduler)
                .DistinctUntilChanged()
                .Select(ValidateEditValue)
                //.ObserveOn(RxApp.MainThreadScheduler)
                .ToProperty(this, x => x.Error);
          
    }

    [Reactive]
    private bool _hasBeenEdited;
    [Reactive]
    private string _editValue = string.Empty;
    [Reactive]
    private string _label = string.Empty;
    
    [ObservableAsProperty]
    private string _error = string.Empty;
    
    private string ValidateEditValue(string editValue)
    {
        if (HasBeenEdited == false && string.IsNullOrEmpty(editValue))
            return string.Empty;
        HasBeenEdited = true;
        var errors = PropertyDescriptor.Validate(editValue);
        return string.Join(" | ", errors.Select(e => e.Description));
    }
}
