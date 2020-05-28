using System.Threading.Tasks;
using TwitchBot.Models;

namespace TwitchBot.Service
{
    public interface ILuisHandler
    {
        Task<IntentResponse> GetIntent(string utterance);
    } 
}