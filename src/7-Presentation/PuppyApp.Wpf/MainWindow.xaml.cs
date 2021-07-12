using PuppyApp.Wpf.Mappers;
using PuppyApp.Wpf.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace PuppyApp.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var sampleJsonSchema = File.ReadAllText("Samples\\SampleJSonSchema.json");
            var request = OpenApiToRequestViewModelMapper.MapToRequest(sampleJsonSchema).Result;
            DataContext = request;
            //request.ContinueWith(InitializeRequestDisplay);
        }

        private async Task InitializeRequestDisplay(Task<RequestViewModel> request)
        {
            DataContext = await request;
        }
    }
}
