using Microsoft.Azure.Cosmos.Table;
using System;
using System.Threading.Tasks;

namespace mvp.db
{
    public interface IDataProvider
    {
        Task Migrate();
    }

    public class DataProvider : IDataProvider
    {
        private string _cnnString;

        public DataProvider(string cnnString)
        {
            _cnnString = cnnString;
        }

        public async Task Migrate()
        {

            _ = CreateTableAsync("todos");

        }

        public async Task<CloudTable> CreateTableAsync(string tableName)
        {
            // Retrieve storage account information from connection string.
            CloudStorageAccount storageAccount = CreateStorageAccountFromConnectionString(_cnnString);

            // Create a table client for interacting with the table service
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient(new TableClientConfiguration());

            // Create a table
            CloudTable table = tableClient.GetTableReference(tableName);
            _ = await table.CreateIfNotExistsAsync();

            return table;
        }

        public CloudStorageAccount CreateStorageAccountFromConnectionString(string storageConnectionString)
        {
            try
            {
                var storageAccount = CloudStorageAccount.Parse(storageConnectionString);
                return storageAccount;
            }
            catch (Exception ex) when (ex is FormatException || ex is ArgumentException)
            {
                throw new Exception("Invalid storage account information provided. Please confirm the AccountName and AccountKey are valid in the app.config file - then restart the application.", ex);
            }
        }
    }
}
