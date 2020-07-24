using System;
using Microsoft.Extensions.Options;
using TwitchBot.Common.Models.Configuration;


namespace TwitchBot.Agent.Rules.ChatCommands
{
    public class ProjectRule : ChatBase 
    {
        public ProjectRule(IOptions<ChatConfiguration> chatConfiguration) : base(chatConfiguration){}

        public override bool IsTextMatched(string chatMessage)
        {
            //todo:  evaluate text length, rather than startwith
            return chatMessage.StartsWith("!project", StringComparison.InvariantCultureIgnoreCase);
        }

        public override string ReturnedMessage(TwitchLib.Client.Events.OnMessageReceivedArgs e)
        {
            //return $"I'm working on revealing as many credentials to the internet as possible";
            return $"{base._chatConfiguration.ProjectDescription}";
        }
    }

}
