using System;
using Microsoft.Extensions.Options;
using TwitchBot.Common.Models.Configuration;

namespace TwitchBot.Agent.Rules.ChatCommands
{

    public class BlogRule : ChatBase
    {

        public BlogRule(IOptions<ChatConfiguration> chatConfiguration) : base(chatConfiguration) { }

        public override bool IsTextMatched(string chatMessage)
        {
            return chatMessage.StartsWith("!blog", StringComparison.InvariantCultureIgnoreCase);
        }

        public override string ReturnedMessage(TwitchLib.Client.Events.OnMessageReceivedArgs e)
        {
            return $"{base._chatConfiguration.Blog}";
        }
    }
}
