using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

using TwitchBot.Agent.Services.Interfaces;
using TwitchBot.Common.Models.Chat;
using TwitchBot.Common.Models.Configuration;

namespace TwitchBot.Agent.Services
{
    public class ChatMessageService : IChatMessageService
    {
        private readonly ChatConfiguration _chatConfiguration;
        private readonly LuisChatResponses _luisChatResponses;

        public ChatMessageService(
            IOptions<LuisChatResponses> luisChatResponses,
            IOptions<ChatConfiguration> chatConfiguration
            )
        {
            _luisChatResponses = luisChatResponses.Value ?? throw new ArgumentNullException(nameof(luisChatResponses));
            _chatConfiguration = chatConfiguration.Value ?? throw new ArgumentNullException(nameof(chatConfiguration));


        }


        public string HandleBotCommands(string chatMessage)
        {
            if (chatMessage.StartsWith("!project", StringComparison.InvariantCultureIgnoreCase))
            {
                return $"We're working on {_chatConfiguration.ProjectDescription}.";
            }

            if (chatMessage.StartsWith("!instagram", StringComparison.InvariantCultureIgnoreCase))
            {
                return $"Follow me on Instagram: {_chatConfiguration.Instagram}";
            }

            if (chatMessage.StartsWith("!twitter", StringComparison.InvariantCultureIgnoreCase))
            {
                return $"Follow me on Twitter: {_chatConfiguration.Twitter}";
            }

            if (chatMessage.StartsWith("!blog", StringComparison.InvariantCultureIgnoreCase))
            {
                return $"My blog: {_chatConfiguration.Blog}";
            }

            if (chatMessage.StartsWith("!playlist", StringComparison.InvariantCultureIgnoreCase))
            {
                return $"Playlist for my live on Twitch: {_chatConfiguration.SpotifyPlaylist}";
            }

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

