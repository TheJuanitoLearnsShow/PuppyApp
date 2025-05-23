using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using PuppyApp.ReactiveViewModels;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace PuppyApp.WinUI3.Controls
{
    public abstract class NumericParameterControlBase : ReactiveUserControl<CallParameterViewModel>
    {

    }
    public sealed partial class NumericParameterControl : NumericParameterControlBase
    {
        public NumericParameterControl()
        {
            InitializeComponent();
            this.WhenActivated(disposableRegistration =>
            {
                ViewModel = this.DataContext as CallParameterViewModel;
                // Notice we don't have to provide a converter, on WPF a global converter is
                // registered which knows how to convert a boolean into visibility.
                this.OneWayBind(ViewModel,
                    viewModel => viewModel.Label,
                    view => view.LabelTxt.Text)
                    .DisposeWith(disposableRegistration);

                //this.Bind(ViewModel,
                //    viewModel => viewModel.EditValue
                //    ,
                //    view => view.EditValueTxt.Text)
                //    .DisposeWith(disposableRegistration);
                this.Bind(ViewModel,
                    viewModel => viewModel.EditValue,
                    view => view.EditValueTxt.Text)
                    .DisposeWith(disposableRegistration);

                this.OneWayBind(ViewModel,
                    viewModel => viewModel.Error,
                    view => view.ErrorTxt.Text)
                    .DisposeWith(disposableRegistration);
            });
        }
    }
}
