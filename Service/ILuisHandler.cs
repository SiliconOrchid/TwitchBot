using System;
using System.Threading.Tasks;

namespace TwitchBot.Service
{
    public interface ILuisHandler
    {
        Task<string> MakeRequest(string utterance);
        Tuple<string,string> ParseResponse(string json);
    } 
}