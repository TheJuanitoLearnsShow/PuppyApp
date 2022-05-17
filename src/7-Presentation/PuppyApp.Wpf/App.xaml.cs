using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace PuppyApp.Wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            this.Activated += StartElmish;
        }

        private void StartElmish(object sender, EventArgs e)
        {
            this.Activated -= StartElmish;
            var connStr = @"Data Source=.\sqlexpress;Database=SampleDb;Integrated Security=True;Pooling=False;MultipleActiveResultSets=True;Connect Timeout=60;";
            SpRequestMvu.main(MainWindow, connStr);
        }
    }
}
