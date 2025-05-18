using System.Collections.ObjectModel;
using ReactiveUI;

namespace PuppyApp.ReactiveViewModels;

public partial class RequestViewModel : ReactiveObject
{
    public ObservableCollection<CallParameterViewModel> CallParameters { get; set; }
}
