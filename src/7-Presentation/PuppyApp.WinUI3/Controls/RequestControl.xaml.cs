using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace PuppyApp.WinUI3.Controls
{
    public sealed partial class RequestControl : UserControl
    {
        public RequestControl()
        {
            this.InitializeComponent();
            var request = new Puppy.SqlViewModels.SpParameterViewModel(connStr, "spEnrollStudent");
            var spParams = [ new ];
            request.LoadNewCallParameters(spParams.Select(p => new Puppy.SqlViewModels.SpParameterViewModel(p, string.Empty, connStr)));
            DataContext = request;
        }
    }
}
