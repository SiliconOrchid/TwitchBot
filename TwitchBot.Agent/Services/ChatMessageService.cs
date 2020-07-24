using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using TwitchBot.Agent.Rules;
using TwitchBot.Agent.Services.Interfaces;
using TwitchBot.Common.Models.Chat;
using TwitchBot.Common.Models.Configuration;

namespace TwitchBot.Agent.Services
{
    public class ChatMessageService : IChatMessageService
    {
        private readonly IEnumerable<IChatMessageRule> _listChatMessageRules;
        private readonly ChatConfiguration _chatConfiguration;
        private readonly LuisChatResponses _luisChatResponses;

        public ChatMessageService(
            IOptions<LuisChatResponses> luisChatResponses,
            IOptions<ChatConfiguration> chatConfiguration,
            IEnumerable<IChatMessageRule> listChatMessageRules
            )
        {
            _luisChatResponses = luisChatResponses.Value ?? throw new ArgumentNullException(nameof(luisChatResponses));
            _chatConfiguration = chatConfiguration.Value ?? throw new ArgumentNullException(nameof(chatConfiguration));
            _listChatMessageRules = listChatMessageRules.ToArray() ?? throw new ArgumentNullException(nameof(listChatMessageRules));

        }


        public string HandleBotCommands(TwitchLib.Client.Events.OnMessageReceivedArgs e)
        {
            IChatMessageRule chatmessageRule = _listChatMessageRules.FirstOrDefault(rule => rule.IsTextMatched(e.ChatMessage.Message.Trim()));

            if (chatmessageRule != null)
            {
                return chatmessageRule.ReturnedMessage(e);
            }


            //if (e.ChatMessage.Message.StartsWith("!uptime", StringComparison.InvariantCultureIgnoreCase))
            //{
            //    var upTime = GetUpTime().Result;
            //    _twitchLibClient.SendMessage(_twitchConfiguration.ChannelName, upTime?.ToString() ?? "Offline");
            //}

            return string.Empty;
        }




        public string MapLuisIntentToResponse(IntentResponse intentResponse)
        {
            string intent = intentResponse.Intent.ToLower();

            switch (intent)
            {
                case "compliment":
                    return _luisChatResponses.Compliment;

                case "greeting":
                    return _luisChatResponses.Greeting;

                case "hostile":
                    return _luisChatResponses.Hostile;

                case "howdoesbotwork":
                    return _luisChatResponses.Howdoesbotwork;

                case "howlongprogramming":
                    return _luisChatResponses.Howlongprogramming;

                case "lowlongstream":
                    return _luisChatResponses.Howlongstream;

                case "innapropriate":
                    return _luisChatResponses.Innapropriate;

                case "provideurllink":
                    return _luisChatResponses.Provideurllink;

                case "whatareyoudoing":
                    return _luisChatResponses.Whatareyoudoing;

                case "whatlanguage":
                    return _luisChatResponses.Whatlanguage;

                case "whendoyoustream":
                    return _luisChatResponses.Whendoyoustream;

                case "whichide":
                    return _luisChatResponses.WhichIDE;

                case "whoareyou":
                    return _luisChatResponses.Whoareyou;

                default:
                    return string.Empty;

            }
        }
    }
}

