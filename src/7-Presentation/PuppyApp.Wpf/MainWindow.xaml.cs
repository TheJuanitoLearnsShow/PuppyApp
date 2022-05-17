using PuppyApp.Wpf.Mappers;
using PuppyApp.Wpf.ViewModels;
using PuppySqlWrapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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

            //var connStr = @"Data Source=.\sqlexpress;Database=SampleDb;Integrated Security=True;Pooling=False;MultipleActiveResultSets=True;Connect Timeout=60;";
            //using var conn = new SqlConnection(connStr);
            //conn.Open();
            //var spParams = PuppyData.SqlMapper.StoredProcProcessor.GetParamHelpersAsTask(conn, "spEnrollStudent").Result;
            //var request = new RequestViewModel(connStr, "spEnrollStudent");
            //request.LoadNewCallParameters(spParams.Select(p => new Puppy.SqlViewModels.SpParameterViewModel(p, string.Empty, connStr)));
            //DataContext = request;
        }

        private async Task InitializeRequestDisplay(Task<RequestViewModel> request)
        {
            DataContext = await request;
        }
    }
}
