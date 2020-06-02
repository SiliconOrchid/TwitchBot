using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.Options;

using TwitchLib.Api;
using TwitchLib.Client;
using TwitchLib.Client.Models;

using TwitchBot.Models;

namespace  TwitchBot.Service
{
    public class TwitchChatBotService : ITwitchChatBotService
    {
        private readonly TwitchConfiguration _twitchConfiguration;
        private readonly ChatConfiguration _chatConfiguration;
        private readonly LuisChatResponses _luisChatResponses;
        private readonly ILuisService _luisHandler;

        private readonly TwitchClient _twitchClient;


        private readonly ConnectionCredentials _connectionCredentials;


        private readonly TwitchAPI twitchAPI = new TwitchAPI();

        private string[] BotUsers = new string[] { "SO-Bot", "streamelements" }; // maintain a list of known bots, so we don't try and interact with them

        private List<string> UsersOnline = new List<string>();

        public TwitchChatBotService(
            IOptions<TwitchConfiguration> twitchConfiguration,
            IOptions<ChatConfiguration> chatConfiguration,
            IOptions<LuisChatResponses> luisChatResponses,
            ILuisService luisHandler
            )
        {
            _twitchConfiguration = twitchConfiguration.Value ?? throw new ArgumentNullException(nameof(twitchConfiguration));
            _chatConfiguration = chatConfiguration.Value ?? throw new ArgumentNullException(nameof(chatConfiguration));
            _luisChatResponses = luisChatResponses.Value ?? throw new ArgumentNullException(nameof(luisChatResponses));

            _luisHandler = luisHandler ?? throw new ArgumentNullException(nameof(luisHandler));

            _connectionCredentials = new ConnectionCredentials(_twitchConfiguration.BotUserName, _twitchConfiguration.BotToken);

            _twitchClient  = new TwitchClient();
        }


        private void InizializeBot()
        {
            _twitchClient.OnLog += Client_OnLog;
            _twitchClient.OnConnectionError += Client_OnConnectionError;
            _twitchClient.OnMessageReceived += Client_OnMessageReceived;
            _twitchClient.OnWhisperReceived += Client_OnWhisperReceived;
            _twitchClient.OnUserTimedout += Client_OnUserTimedout;
            _twitchClient.OnNewSubscriber += Client_OnNewSubscriber;
            _twitchClient.OnUserJoined += Client_OnUserJoined;
            _twitchClient.OnUserLeft += Client_OnUserLeft;

            _twitchClient.Initialize(_connectionCredentials, _twitchConfiguration.ChannelName);
            _twitchClient.Connect();

            _twitchClient.OnConnected += Client_OnConnected;
        }

        private void Client_OnConnected(object sender, TwitchLib.Client.Events.OnConnectedArgs e)
        {
            _twitchClient.SendMessage(_twitchConfiguration.ChannelName, $"Hi to everyone.");
        }

        private void Client_OnNewSubscriber(object sender, TwitchLib.Client.Events.OnNewSubscriberArgs e)
        {
            _twitchClient.SendMessage(_twitchConfiguration.ChannelName, $"Thank you for the subscription {e.Subscriber.DisplayName}!!! I really appreciate it!");
        }

        private void Client_OnUserTimedout(object sender, TwitchLib.Client.Events.OnUserTimedoutArgs e)
        {
            _twitchClient.SendMessage(_twitchConfiguration.ChannelName, $"User {e.UserTimeout.Username} timed out.");
        }

        private void Client_OnWhisperReceived(object sender, TwitchLib.Client.Events.OnWhisperReceivedArgs e)
        {
            //_twitchClient.SendWhisper(e.WhisperMessage.Username, $"your said: { e.WhisperMessage.Message}");
        }

        private void Client_OnMessageReceived(object sender, TwitchLib.Client.Events.OnMessageReceivedArgs e)
        {
            if (e.ChatMessage.Message.StartsWith("!uptime", StringComparison.InvariantCultureIgnoreCase))
            {
                var upTime = GetUpTime().Result;
                _twitchClient.SendMessage(_twitchConfiguration.ChannelName, upTime?.ToString() ?? "Offline");
            }
            else if (e.ChatMessage.Message.StartsWith("!project", StringComparison.InvariantCultureIgnoreCase))
            {
                _twitchClient.SendMessage(_twitchConfiguration.ChannelName, $"We're working on {_chatConfiguration.ProjectDescription}.");
            }
            else if (e.ChatMessage.Message.StartsWith("!instagram", StringComparison.InvariantCultureIgnoreCase))
            {
                _twitchClient.SendMessage(_twitchConfiguration.ChannelName, $"Follow me on Instagram: {_chatConfiguration.Instagram}");
            }
            else if (e.ChatMessage.Message.StartsWith("!twitter", StringComparison.InvariantCultureIgnoreCase))
            {
                _twitchClient.SendMessage(_twitchConfiguration.ChannelName, $"Follow me on Twitter: {_chatConfiguration.Twitter}");
            }
            else if (e.ChatMessage.Message.StartsWith("!blog", StringComparison.InvariantCultureIgnoreCase))
            {
                _twitchClient.SendMessage(_twitchConfiguration.ChannelName, $"My blog: {_chatConfiguration.Blog}");
            }
            else if (e.ChatMessage.Message.StartsWith("!playlist", StringComparison.InvariantCultureIgnoreCase))
            {
                _twitchClient.SendMessage(_twitchConfiguration.ChannelName, $"Playlist for my live on Twitch: {_chatConfiguration.SpotifyPlaylist}");
            }


            else
            {

                _twitchClient.SendMessage(_twitchConfiguration.ChannelName, HandleLuisChat(e.ChatMessage.Message));
            }
        }

        private string  HandleLuisChat(string chatMessage)
        {
                // Run async method in this sync method  (read https://cpratt.co/async-tips-tricks/)
                IntentResponse intentResponse = AsyncHelper.RunSync(() => _luisHandler.GetIntent(chatMessage));  
           
                string intent = intentResponse.Intent.ToLower();

                //string myintent = "hostile";

                decimal certaintyThreshold = 0.4m;
                
                if (intentResponse.Certainty > certaintyThreshold)
                {
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

                    }
                }

                return string.Empty;

        }


        private async Task<TimeSpan?> GetUpTime()
        {
            var userId = await GetUserId(_twitchConfiguration.ChannelName);

            return await twitchAPI.V5.Streams.GetUptimeAsync(userId);
        }

        private async Task<string> GetUserId(string username)
        {
            var userList = await twitchAPI.V5.Users.GetUserByNameAsync(username);

            return userList.Matches[0].Id;
        }

        private void Client_OnConnectionError(object sender, TwitchLib.Client.Events.OnConnectionErrorArgs e)
        {
            Console.WriteLine(e.Error.Message);
        }

        private void Client_OnLog(object sender, TwitchLib.Client.Events.OnLogArgs e)
        {
            Console.WriteLine(e.Data);
        }

        private void Client_OnUserJoined(object sender, TwitchLib.Client.Events.OnUserJoinedArgs e)
        {
            if (BotUsers.Contains(e.Username)) return;

            try
            {
                //_twitchClient.SendMessage(TwitchInfo.ChannelName, $"Welcome on my channel, { e.Username }.");

                UsersOnline.Add(e.Username);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void Client_OnUserLeft(object sender, TwitchLib.Client.Events.OnUserLeftArgs e)
        {
            UsersOnline.Remove(e.Username);
        }


        public void Connect()
        {
            Console.WriteLine("Connecting to Twitch...");

            twitchAPI.Settings.ClientId = _twitchConfiguration.ClientId;

            InizializeBot();
        }

        public void Disconnect()
        {
            Console.WriteLine("Disconnecting from Twitch...");
        }
    }
}