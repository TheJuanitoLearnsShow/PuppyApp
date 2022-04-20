
using MvvmGen;
using Puppy.SqlViewModels;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PuppyApp.Wpf.ViewModels
{
    [ViewModel]
    internal partial class RequestViewModel
    {
        public ObservableCollection<SpParameterViewModel> CallParameters { get; } = new ObservableCollection<SpParameterViewModel>();

        public void LoadNewCallParameters(IEnumerable<SpParameterViewModel> newItems)
        {
            foreach (var p in newItems)
            {
                CallParameters.Add(p);
            }

            OnPropertyChanged("CallParameters");
        }
    }
}