using TwitchLib.Client.Events;

namespace TwitchBot.Agent.Rules
{
    public interface IChatMessageRule 
    {
        bool IsTextMatched(string chatMessage);
        string ReturnedMessage(OnMessageReceivedArgs e);
    }
}