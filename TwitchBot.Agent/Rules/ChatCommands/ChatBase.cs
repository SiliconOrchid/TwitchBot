using Microsoft.Extensions.Options;
using TwitchLib.Client.Events;
using TwitchBot.Common.Models.Configuration;

namespace TwitchBot.Agent.Rules.ChatCommands
{
    public abstract class ChatBase : IChatMessageRule
    {
        internal readonly ChatConfiguration _chatConfiguration;

        public ChatBase(IOptions<ChatConfiguration> chatConfiguration)
        {
            _chatConfiguration = chatConfiguration.Value;
        }

        public abstract bool IsTextMatched(string chatMessage);

        public abstract string ReturnedMessage(OnMessageReceivedArgs e);


    }
}


