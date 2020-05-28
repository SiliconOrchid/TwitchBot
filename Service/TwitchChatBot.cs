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
    public class TwitchChatBot : ITwitchChatBot
    {
        private TwitchConfiguration _twitchConfiguration;
        private ChatConfiguration _chatConfiguration;


        private readonly ConnectionCredentials _connectionCredentials;
        private TwitchClient client;
        private readonly TwitchAPI twitchAPI = new TwitchAPI();

        private string[] BotUsers = new string[] { "SO-Bot", "streamelements" }; // maintain a list of known bots, so we don't try and interact with them

        private List<string> UsersOnline = new List<string>();

        public TwitchChatBot(
            IOptions<TwitchConfiguration> twitchConfiguration,
            IOptions<ChatConfiguration> chatConfiguration
            )
        {
            _twitchConfiguration = twitchConfiguration.Value ?? throw new ArgumentNullException(nameof(twitchConfiguration));
            _chatConfiguration = chatConfiguration.Value ?? throw new ArgumentNullException(nameof(chatConfiguration));

            _connectionCredentials = new ConnectionCredentials(_twitchConfiguration.BotUserName, _twitchConfiguration.BotToken);
        }


        private void InizializeBot()
        {
            client = new TwitchClient();

            client.OnLog += Client_OnLog;
            client.OnConnectionError += Client_OnConnectionError;
            client.OnMessageReceived += Client_OnMessageReceived;
            client.OnWhisperReceived += Client_OnWhisperReceived;
            client.OnUserTimedout += Client_OnUserTimedout;
            client.OnNewSubscriber += Client_OnNewSubscriber;
            client.OnUserJoined += Client_OnUserJoined;
            client.OnUserLeft += Client_OnUserLeft;

            client.Initialize(_connectionCredentials, _twitchConfiguration.ChannelName);
            client.Connect();

            client.OnConnected += Client_OnConnected;
        }

        private void Client_OnConnected(object sender, TwitchLib.Client.Events.OnConnectedArgs e)
        {
            client.SendMessage(_twitchConfiguration.ChannelName, $"Hi to everyone.");
        }

        private void Client_OnNewSubscriber(object sender, TwitchLib.Client.Events.OnNewSubscriberArgs e)
        {
            client.SendMessage(_twitchConfiguration.ChannelName, $"Thank you for the subscription {e.Subscriber.DisplayName}!!! I really appreciate it!");
        }

        private void Client_OnUserTimedout(object sender, TwitchLib.Client.Events.OnUserTimedoutArgs e)
        {
            client.SendMessage(_twitchConfiguration.ChannelName, $"User {e.UserTimeout.Username} timed out.");
        }

        private void Client_OnWhisperReceived(object sender, TwitchLib.Client.Events.OnWhisperReceivedArgs e)
        {
            //client.SendWhisper(e.WhisperMessage.Username, $"your said: { e.WhisperMessage.Message}");
        }

        private void Client_OnMessageReceived(object sender, TwitchLib.Client.Events.OnMessageReceivedArgs e)
        {
            if (e.ChatMessage.Message.StartsWith("!uptime", StringComparison.InvariantCultureIgnoreCase))
            {
                var upTime = GetUpTime().Result;
                client.SendMessage(_twitchConfiguration.ChannelName, upTime?.ToString() ?? "Offline");
            }
            else if (e.ChatMessage.Message.StartsWith("!project", StringComparison.InvariantCultureIgnoreCase))
            {
                client.SendMessage(_twitchConfiguration.ChannelName, $"We're working on {_chatConfiguration.ProjectDescription}.");
            }
            else if (e.ChatMessage.Message.StartsWith("!instagram", StringComparison.InvariantCultureIgnoreCase))
            {
                client.SendMessage(_twitchConfiguration.ChannelName, $"Follow me on Instagram: {_chatConfiguration.Instagram}");
            }
            else if (e.ChatMessage.Message.StartsWith("!twitter", StringComparison.InvariantCultureIgnoreCase))
            {
                client.SendMessage(_twitchConfiguration.ChannelName, $"Follow me on Twitter: {_chatConfiguration.Twitter}");
            }
            else if (e.ChatMessage.Message.StartsWith("!blog", StringComparison.InvariantCultureIgnoreCase))
            {
                client.SendMessage(_twitchConfiguration.ChannelName, $"My blog: {_chatConfiguration.Blog}");
            }
            else if (e.ChatMessage.Message.StartsWith("!playlist", StringComparison.InvariantCultureIgnoreCase))
            {
                client.SendMessage(_twitchConfiguration.ChannelName, $"Playlist for my live on Twitch: {_chatConfiguration.SpotifyPlaylist}");
            }


            else
            {

            }
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
                //client.SendMessage(TwitchInfo.ChannelName, $"Welcome on my channel, { e.Username }.");

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