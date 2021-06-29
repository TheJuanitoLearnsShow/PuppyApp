using PuppyApp.Wpf.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PuppyApp.Wpf.Controls
{
    /// <summary>
    /// Interaction logic for CallParametersControl.xaml
    /// </summary>
    public partial class CallParametersControl : UserControl
    {
        public CallParametersControl()
        {
            InitializeComponent();
            this.DataContext = new CallParameterViewModel() { Label ="Name" } ;
        }
    }
}
