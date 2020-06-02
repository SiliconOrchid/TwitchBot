
using System.Threading.Tasks;

using TwitchBot.Models.ExtractedData;

namespace TwitchBot.Service
{
    public interface ICosmosDbService
    {

        Task StartupDatabase();

        Task AddItemsToContainerAsync(ChatLink chatLink);
    }
}
