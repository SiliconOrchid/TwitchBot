using System.Threading.Tasks;

using TwitchBot.Common.Models.Chat;

namespace TwitchBot.Agent.Services.Interfaces
{
    public interface ILuisService
    {
        Task<IntentResponse> GetIntentAsync(string utterance);
    }
}