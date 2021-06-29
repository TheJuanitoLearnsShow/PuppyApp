using PuppySqlWrapper;
using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace PuppyTests
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using FileStream fs = File.Create("test.json");
            using var jsonWriter = new Utf8JsonWriter(fs);
            var connStr = "Persist Security Info=False;Integrated Security=true;Initial Catalog=HoaAccounting; Server=.\\sqlexpress";
            var sqlQuery = @"SELECT TOP (1000) [GLTransactionId]
      ,[AccountId]
      ,[CreditDebitFlag]
      ,[Description]
      ,[ReportDescription]
      ,[TransactionDate]
      ,[BillPayConfirmation]
      ,[CheckNumber]
      ,[Amount]
      ,[TranCount]
      ,[GLCategory]
      ,[CreatedOn]
      ,[ExcludeFromReport]
  FROM [HoaAccounting].[dbo].[GLTransactions]";
            await SqlResultAsJsonProcessor.WriteSqlQueryResultAsJsonAsync(connStr, sqlQuery, jsonWriter);
            Console.WriteLine("Done");
        }
    }
}
