using System.Threading.Tasks;

namespace TwitchBot.Service
{
    public interface ITwitchChatBot
    {
        void Connect();
        void Disconnect();
    } 
}