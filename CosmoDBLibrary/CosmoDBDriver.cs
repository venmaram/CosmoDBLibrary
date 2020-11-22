using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using ViewModel;

namespace CosmoDBLibrary
{
    public class CosmoDBDriver
    {
        // The Azure Cosmos DB endpoint for running this sample.
        private static readonly string EndpointUri = "https://localhost:8081";

        // The primary key for the Azure Cosmos account.
        private static readonly string PrimaryKey = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";

        // The Cosmos client instance
        private CosmosClient cosmosClient;

        // The database we will create
        private Database database;

        // The container we will create.
        private Container container;

        // The name of the database and container we will create
        private string databaseId = "Tasks";
        private string containerId = "event";

        public  CosmoDBDriver()
        {
            //connection();
        }
        private async void connection()
        {
            // Create a new instance of the Cosmos Client
            this.cosmosClient = new CosmosClient(EndpointUri, PrimaryKey, new CosmosClientOptions() { ApplicationName = "CosmosDBDotnetQuickstart" });
            await this.CreateDatabaseAsync();
            await this.CreateContainerAsync();
            await this.ScaleContainerAsync();


        }

       

        private async Task CreateDatabaseAsync()
        {
            // Create a new database
            this.database = await this.cosmosClient.CreateDatabaseIfNotExistsAsync(databaseId);
        }
        private async Task ScaleContainerAsync()
        {
            // Read the current throughput
            int? throughput = await this.container.ReadThroughputAsync();
            if (throughput.HasValue)
            {
                int newThroughput = throughput.Value + 100;
                // Update throughput
                await this.container.ReplaceThroughputAsync(newThroughput);
            }

        }
        private async Task CreateContainerAsync()
        {
            // Create a new container
            this.container = await this.database.CreateContainerIfNotExistsAsync(containerId, "/Name", 400);
        }


        /// <summary>
        ///  Add  items to the container
        /// </summary>
        /// <returns></returns>
        public async Task AddItemsToContainerAsync(Event eve)
        {
            this.cosmosClient = new CosmosClient(EndpointUri, PrimaryKey, new CosmosClientOptions() { ApplicationName = "CosmosDBDotnetQuickstart" });
            this.database = await this.cosmosClient.CreateDatabaseIfNotExistsAsync(databaseId);
            this.container = await this.database.CreateContainerIfNotExistsAsync(containerId, "/Name", 400);
            try
            {
                ItemResponse<Event> eve_Response = await this.container.ReadItemAsync<Event>(eve.Id, new PartitionKey(eve.Name));

            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                ItemResponse<Event> eve_Response = await this.container.CreateItemAsync<Event>(eve, new PartitionKey(eve.Name));
            }

            Event eve1 = new Event
            {
                Id = "Diyan.1",
                Name = "Diyan",
                Description = "event client"
            };

            try
            {
                ItemResponse<Event> eve1_Response = await this.container.ReadItemAsync<Event>(eve1.Id, new PartitionKey(eve1.Name));

            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                ItemResponse<Event> eve1_Response = await this.container.CreateItemAsync<Event>(eve1, new PartitionKey(eve1.Name));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<List<Event>> ReadItemsAsync(string name)
        {
            this.cosmosClient = new CosmosClient(EndpointUri, PrimaryKey, new CosmosClientOptions() { ApplicationName = "CosmosDBDotnetQuickstart" });
            this.database = await this.cosmosClient.CreateDatabaseIfNotExistsAsync(databaseId);
            this.container = await this.database.CreateContainerIfNotExistsAsync(containerId, "/Name", 400);
            var sqlQueryText = "SELECT * FROM c WHERE c.Name = '"+ name + "'";


            QueryDefinition queryDefinition = new QueryDefinition(sqlQueryText);
            FeedIterator<Event> queryResultSetIterator = this.container.GetItemQueryIterator<Event>(queryDefinition);

            List<Event> events = new List<Event>();

            while (queryResultSetIterator.HasMoreResults)
            {
                FeedResponse<Event> currentResultSet = await queryResultSetIterator.ReadNextAsync();
                foreach (Event eves in currentResultSet)
                {
                    events.Add(eves);
                }
            }
            return events;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task UpdateItemAsync(string partitionKeyValue, string Id)
        {
            this.cosmosClient = new CosmosClient(EndpointUri, PrimaryKey, new CosmosClientOptions() { ApplicationName = "CosmosDBDotnetQuickstart" });
            this.database = await this.cosmosClient.CreateDatabaseIfNotExistsAsync(databaseId);
            this.container = await this.database.CreateContainerIfNotExistsAsync(containerId, "/Name", 400);
            ItemResponse<Event> wakefieldFamilyResponse = await this.container.ReadItemAsync<Event>(Id, new PartitionKey(partitionKeyValue));
            var itemBody = wakefieldFamilyResponse.Resource;

            itemBody.Description = "Update the event description";

            wakefieldFamilyResponse = await this.container.ReplaceItemAsync<Event>(itemBody, itemBody.Id, new PartitionKey(itemBody.Name));
        }

        /// <summary>
        /// Delete item based on id and key
        /// </summary>
        /// <param name="partitionKeyValue"></param>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task DeleteItemAsync(string partitionKeyValue,string Id)
        {
            this.cosmosClient = new CosmosClient(EndpointUri, PrimaryKey, new CosmosClientOptions() { ApplicationName = "CosmosDBDotnetQuickstart" });
            this.database = await this.cosmosClient.CreateDatabaseIfNotExistsAsync(databaseId);
            this.container = await this.database.CreateContainerIfNotExistsAsync(containerId, "/Name", 400);
           // partitionKeyValue = "Diyan";
            // Id = "Diyan.1";

            // Delete an item. Note we must provide the partition key value and id of the item to delete
            ItemResponse<Event> wakefieldFamilyResponse = await this.container.DeleteItemAsync<Event>(Id, new PartitionKey(partitionKeyValue));
        }

        /// <summary>
        /// delete the entire database
        /// </summary>
        /// <returns></returns>
        public async Task DeleteDatabaseAndCleanupAsync()
        {
            this.cosmosClient = new CosmosClient(EndpointUri, PrimaryKey, new CosmosClientOptions() { ApplicationName = "CosmosDBDotnetQuickstart" });
            this.database = await this.cosmosClient.CreateDatabaseIfNotExistsAsync(databaseId);
            this.container = await this.database.CreateContainerIfNotExistsAsync(containerId, "/Name", 400);
            DatabaseResponse databaseResourceResponse = await this.database.DeleteAsync();

            //Dispose of CosmosClient
            this.cosmosClient.Dispose();
        }

        public static string Serialize<T>(T obj)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
            MemoryStream ms = new MemoryStream();
            serializer.WriteObject(ms, obj);
            string retVal = Encoding.UTF8.GetString(ms.ToArray());
            return retVal;
        }

        public static T Deserialize<T>(string json)
        {
            T obj = Activator.CreateInstance<T>();
            MemoryStream ms = new MemoryStream(Encoding.Unicode.GetBytes(json));
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
            obj = (T)serializer.ReadObject(ms);
            ms.Close();
            return obj;
        }








    }
}
