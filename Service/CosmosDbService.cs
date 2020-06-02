namespace TwitchBot.Service
{
    public class CosmosDbService : ICosmosDbService
    {
        // The Azure Cosmos DB endpoint for running this sample.
        private static readonly string _endpointUri;
        // The primary key for the Azure Cosmos account.
        private static readonly string _primaryKey;

        // The Cosmos client instance
        private CosmosClient _cosmosClient;

        // The database we will create
        private Database _database;

        // The container we will create.
        private Container _container;

        // The name of the database and container we will create
        private string databaseId = "FamilyDatabase";
        private string containerId = "FamilyContainer";


        public LuisService(IOptions<CosmosDbConfiguration> cosmosDbConfiguration)
        {
            _cosmosDbConfiguration = cosmosDbConfiguration.Value ?? throw new ArgumentNullException(nameof(cosmosDbConfiguration));


        }



    }
}