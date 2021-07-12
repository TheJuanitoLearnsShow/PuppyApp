
using MvvmGen;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PuppyApp.Wpf.ViewModels
{
    [ViewModel]
    internal partial class RequestViewModel
    {
        public ObservableCollection<CallParameterViewModel> CallParameters { get; } = new ObservableCollection<CallParameterViewModel>();

        public void LoadNewCallParameters(IEnumerable<CallParameterViewModel> newItems)
        {
            foreach (var p in newItems)
            {
                CallParameters.Add(p);
            }

            OnPropertyChanged("CallParameters");
        }
    }
}