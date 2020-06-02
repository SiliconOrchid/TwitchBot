using System.Threading.Tasks;
using TwitchBot.Models;

namespace TwitchBot.Service
{
    public interface ILuisService
    {
        Task<IntentResponse> GetIntent(string utterance);
    } 
}