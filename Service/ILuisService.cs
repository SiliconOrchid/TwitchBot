using System.Threading.Tasks;
using TwitchBot.Models.Chat;

namespace TwitchBot.Service
{
    public interface ILuisService
    {
        Task<IntentResponse> GetIntent(string utterance);
    } 
}