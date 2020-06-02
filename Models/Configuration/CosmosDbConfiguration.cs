namespace TwitchBot.Models.Configuration
{
    public class CosmosDbConfiguration
    {
        public string EndpointUri { get; set; }

        public string AuthorizationKey { get; set; }

        public string DatabaseId { get; set; }

        public string ContainerId { get; set; }

        public string PartitionKeyPath { get; set; }

    }
}
