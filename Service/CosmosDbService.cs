
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;


using Azure.Cosmos;
using Microsoft.Extensions.Options;

using TwitchBot.Models.Chat;
using TwitchBot.Models.Configuration;
using TwitchBot.Models.ExtractedData;

namespace TwitchBot.Service
{
    public class CosmosDbService : ICosmosDbService
    {
        // using reference : https://docs.microsoft.com/en-us/azure/cosmos-db/create-sql-api-dotnet-v4

        private readonly CosmosClient _cosmosClient;  // The Cosmos client instance

        private readonly CosmosDbConfiguration _cosmosDbConfiguration;

        //private Database _database; // The database 
        //private Container _container; // The container
        //private string _databaseId; // The name of the database and container
        //private string _containerId;


        public CosmosDbService(IOptions<CosmosDbConfiguration> cosmosDbConfiguration)
        {
            _cosmosDbConfiguration = cosmosDbConfiguration.Value ?? throw new ArgumentNullException(nameof(cosmosDbConfiguration));

            _cosmosClient = new CosmosClient(_cosmosDbConfiguration.EndpointUri, _cosmosDbConfiguration.AuthorizationKey);
        }

        //public async Task DoStuff()
        //{

        //    await CreateDatabaseAsync();
        //    await CreateContainerAsync();
        //    await AddItemsToContainerAsync();
        //    await QueryItemsAsync();
        //}

        public async Task StartupDatabase()
        {
            await CreateDatabaseAsync();
            await CreateContainerAsync();
        }



        /// <summary>
        /// Create the database if it does not exist
        /// </summary>
        private async Task CreateDatabaseAsync()
        {
            // Create a new database
            CosmosDatabase database = await _cosmosClient.CreateDatabaseIfNotExistsAsync(_cosmosDbConfiguration.DatabaseId);
            Console.WriteLine("Created Database: {0}\n", database.Id);
        }

        /// <summary>
        /// Create the container if it does not exist. 
        /// Specify "/LastName" as the partition key since we're storing family information, to ensure good distribution of requests and storage.
        /// </summary>
        /// <returns></returns>
        private async Task CreateContainerAsync()
        {
            // Create a new container
            CosmosContainer container = await _cosmosClient.GetDatabase(_cosmosDbConfiguration.DatabaseId).CreateContainerIfNotExistsAsync(_cosmosDbConfiguration.ContainerId, $"/{_cosmosDbConfiguration.PartitionKeyPath}");
            Console.WriteLine("Created Container: {0}\n", container.Id);
        }


        /// <summary>
        /// Add items to the container
        /// </summary>
        public async Task AddItemsToContainerAsync(ChatLink chatLink)
        {

            //ChatLink chatLink = new ChatLink
            //{
            //    Id = Guid.NewGuid().ToString(),
            //    TwitchUserName = "MrTest",
            //    SharedUrl = "https://blogs.siliconorchid.com",
            //    DateShared = DateTime.UtcNow
            //};

            CosmosContainer container = _cosmosClient.GetContainer(_cosmosDbConfiguration.DatabaseId, _cosmosDbConfiguration.ContainerId);
            try
            {
                // Read the item to see if it exists.  
                ItemResponse<ChatLink> chatLinkResponse = await container.ReadItemAsync<ChatLink>(chatLink.Id, new PartitionKey(chatLink.TwitchUserName));
                Console.WriteLine("Item in database with id: {0} already exists\n", chatLinkResponse.Value.Id);
            }
            catch (CosmosException ex) when (ex.Status == (int)HttpStatusCode.NotFound)
            {
                // Create an item in the container representing the Andersen family. Note we provide the value of the partition key for this item, which is "Andersen"
                ItemResponse<ChatLink> chatLinkResponse = await container.CreateItemAsync<ChatLink>(chatLink, new PartitionKey(chatLink.TwitchUserName));

                // Note that after creating the item, we can access the body of the item with the Resource property off the ItemResponse.
                Console.WriteLine("Created item in database with id: {0}\n", chatLinkResponse.Value.Id);
            }
        }



        /// <summary>
        /// Run a query (using Azure Cosmos DB SQL syntax) against the container
        /// </summary>
        private async Task QueryItemsAsync()
        {
            var sqlQueryText = "SELECT * FROM c WHERE c.TwitchUserName = 'MrTest'";

            Console.WriteLine("Running query: {0}\n", sqlQueryText);

            CosmosContainer container = _cosmosClient.GetContainer(_cosmosDbConfiguration.DatabaseId, _cosmosDbConfiguration.ContainerId);

            QueryDefinition queryDefinition = new QueryDefinition(sqlQueryText);

            List<ChatLink> chatLinks = new List<ChatLink>();

            await foreach (ChatLink chatLink in container.GetItemQueryIterator<ChatLink>(queryDefinition))
            {
                chatLinks.Add(chatLink);
                Console.WriteLine("\tRead {0}\n", chatLink);
            }
        }



    }
}