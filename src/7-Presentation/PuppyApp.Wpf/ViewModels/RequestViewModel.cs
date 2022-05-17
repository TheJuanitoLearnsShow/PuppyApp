
using MvvmGen;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace PuppyApp.Wpf.ViewModels
{

    [ViewModel]
    internal partial class RequestViewModel
    {
        private readonly string _connectionString;
        private readonly string _spName;

        public DataTable Results { get; set; }

        public RequestViewModel(string connectionString, string spName)
        {
            _connectionString = connectionString;
            _spName = spName;
            Results = new DataTable();
            //this.InitializeCommands();
            this.OnInitialize();
        }

        //public ObservableCollection<SpParameterViewModel> CallParameters { get; } = new ObservableCollection<SpParameterViewModel>();

        //public void LoadNewCallParameters(IEnumerable<SpParameterViewModel> newItems)
        //{
        //    foreach (var p in newItems)
        //    {
        //        (p as INotifyPropertyChanged).PropertyChanged += OnParameterChanged;
        //        CallParameters.Add(p);
        //    }

        //    OnPropertyChanged("CallParameters");
        //}

        private void OnParameterChanged(object sender, PropertyChangedEventArgs e)
        {
            //ExecuteCommand.RaiseCanExecuteChanged();
        }

        //[Command(CanExecuteMethod = nameof(CanExecute))]
        private void Execute() {
            using SqlConnection connection = new SqlConnection(_connectionString);

            using SqlCommand cmd = new SqlCommand(_spName, connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            //foreach(var p in CallParameters)
            //{
            //    cmd.Parameters.AddWithValue(p.SpParamName, p.Value);
            //}
            connection.Open();
            Results.Clear();
            using var adapter = new SqlDataAdapter(cmd);
            {
                adapter.Fill(Results);
            }
        }

        //private bool CanExecute()
        //{
        //    return CallParameters.All( 
        //        p => p.IsValid
        //        );
        //}
    }
}