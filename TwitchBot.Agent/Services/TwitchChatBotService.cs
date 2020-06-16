using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.Options;

using TwitchLib.Api;
using TwitchLib.Client;
using TwitchLib.Client.Models;
using TwitchLib.Client.Events;


using TwitchBot.Agent.Services.Interfaces;
using TwitchBot.Common.Models.Chat;
using TwitchBot.Common.Models.Configuration;
using TwitchBot.Common.Models.ExtractedData;
using TwitchBot.Common.Data.Repository.Interfaces;
using TwitchBot.Common.Models.Entity;

namespace TwitchBot.Agent.Services
{
    public class TwitchChatBotService : ITwitchChatBotService
    {
        private readonly ConnectionCredentials _twitchClientConnectionCredentials;
        private readonly TwitchConfiguration _twitchConfiguration;
        private readonly LuisConfiguration _luisConfiguration;

        private readonly IChatMessageService _chatMessageService;
        private readonly ITwitchUserChatRepository _twitchUserChatRepository;
        private readonly ITwitchNewSubscriberRepository _twitchNewSubscriberRepository;
        private readonly ITwitchUserAttendanceEventRepository _twitchUserAttendanceEventRepository;
        private readonly ILuisService _luisService;

        private readonly TwitchClient _twitchClient;


        private readonly TwitchAPI twitchAPI = new TwitchAPI();

        private string[] BotUsers = new string[] { "SO-Bot", "streamelements" }; // maintain a list of known bots, so we don't try and interact with them

        private List<string> UsersOnline = new List<string>();

        public TwitchChatBotService(
            IOptions<TwitchConfiguration> twitchConfiguration,
            IOptions<LuisConfiguration> luisConfiguration,
            IChatMessageService chatMessageService,
            ITwitchUserChatRepository twitchUserChatRepository,
            ITwitchNewSubscriberRepository twitchNewSubscriberRepository,
            ITwitchUserAttendanceEventRepository twitchUserAttendanceEventRepository,
            ILuisService luisService
            )
        {
            _twitchConfiguration = twitchConfiguration.Value ?? throw new ArgumentNullException(nameof(twitchConfiguration));
            _luisConfiguration = luisConfiguration.Value ?? throw new ArgumentNullException(nameof(luisConfiguration));
            _chatMessageService = chatMessageService ?? throw new ArgumentNullException(nameof(chatMessageService));
            _twitchUserChatRepository = twitchUserChatRepository ?? throw new ArgumentNullException(nameof(twitchUserChatRepository));
            _twitchNewSubscriberRepository = twitchNewSubscriberRepository ?? throw new ArgumentNullException(nameof(twitchNewSubscriberRepository));
            _twitchUserAttendanceEventRepository = twitchUserAttendanceEventRepository ?? throw new ArgumentNullException(nameof(twitchUserAttendanceEventRepository));
            _twitchClientConnectionCredentials = new ConnectionCredentials(_twitchConfiguration.BotUserName, _twitchConfiguration.BotToken);
            _luisService = luisService ?? throw new ArgumentNullException(nameof(luisService));
            _twitchClient = new TwitchClient();
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

            _twitchClient.Initialize(_twitchClientConnectionCredentials, _twitchConfiguration.ChannelName);
            _twitchClient.Connect();

            _twitchClient.OnConnected += Client_OnConnected;
        }

        private void Client_OnConnected(object sender, TwitchLib.Client.Events.OnConnectedArgs e)
        {
            _twitchClient.SendMessage(_twitchConfiguration.ChannelName, $"Hi to everyone.");
        }

        private void Client_OnNewSubscriber(object sender, TwitchLib.Client.Events.OnNewSubscriberArgs e)
        {
            AddSubscriberRecordToDb(e);

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
            string botCommandResponse = _chatMessageService.HandleBotCommands(e.ChatMessage.Message.Trim());

            if (string.IsNullOrEmpty(botCommandResponse))  // do something with luis
            {
                // Run async method in this sync method  (read https://cpratt.co/async-tips-tricks/)
                IntentResponse intentResponse = AsyncHelper.RunSync(() => _luisService.GetIntentAsync(e.ChatMessage.Message.Trim()));

                decimal certaintyThreshold;

                if (!decimal.TryParse(_luisConfiguration.LuisChatCertaintyThreshold, out certaintyThreshold))
                {
                    throw new ArgumentException(nameof(_luisConfiguration.LuisChatCertaintyThreshold));
                }


                if (intentResponse.Certainty > certaintyThreshold)
                {
                    string luisMappedResponse = _chatMessageService.MapLuisIntentToResponse(intentResponse);
                    _twitchClient.SendMessage(_twitchConfiguration.ChannelName, luisMappedResponse);
                }

                AddTwitchUserChatRecordToDb(e, intentResponse);
            }
            else // do something with explicit bot commands
            {
                _twitchClient.SendMessage(_twitchConfiguration.ChannelName, botCommandResponse);
            }

        }


        //private async Task<TimeSpan?> GetUpTime()
        //{
        //    var userId = await GetUserId(_twitchConfiguration.ChannelName);

        //    return await twitchAPI.V5.Streams.GetUptimeAsync(userId);
        //}

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

                LogUserJoinedChatToDb(e);

                UsersOnline.Add(e.Username);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }



        private void Client_OnUserLeft(object sender, TwitchLib.Client.Events.OnUserLeftArgs e)
        {
            LogUserLeftChatToDb(e);

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

        private void AddSubscriberRecordToDb(OnNewSubscriberArgs e)
        {
            TwitchNewSubscriber twitchNewSubscriber = new TwitchNewSubscriber
            {
                TwitchUserDisplayName = e.Subscriber.DisplayName.ToString(),
            };

            AsyncHelper.RunSync(() => _twitchNewSubscriberRepository.CreateAsync(twitchNewSubscriber));
        }

        private void AddTwitchUserChatRecordToDb(OnMessageReceivedArgs e, IntentResponse intentResponse)
        {
            // create new chat record
            TwitchUserChat twitchUserChat = new TwitchUserChat
            {
                TwitchUserId = e.ChatMessage.UserId.ToString(),
                TwitchUserType = e.ChatMessage.UserType.ToString(),
                TwitchUserDisplayName = e.ChatMessage.DisplayName.ToString(),
                ChatMessage = e.ChatMessage.Message,
                ExtractedUrl = intentResponse.EmbeddedUrl,
                LuisIntent = intentResponse.Intent,
                LuisCertainty = intentResponse.Certainty
            };

            // add chat record to DB
            AsyncHelper.RunSync(() => _twitchUserChatRepository.CreateAsync(twitchUserChat));
        }

        private void LogUserJoinedChatToDb(OnUserJoinedArgs e)
        {
            TwitchUserAttendanceEvent twitchUserAttendanceEvent = new TwitchUserAttendanceEvent
            {
                TwitchUserDisplayName = e.Username,
                UserJoined = true
            };

            _twitchUserAttendanceEventRepository.CreateAsync(twitchUserAttendanceEvent);
        }

        private void LogUserLeftChatToDb(OnUserLeftArgs e)
        {
            TwitchUserAttendanceEvent twitchUserAttendanceEvent = new TwitchUserAttendanceEvent
            {
                TwitchUserDisplayName = e.Username,
                UserLeft = true
            };

            _twitchUserAttendanceEventRepository.CreateAsync(twitchUserAttendanceEvent);
        }

    }
}