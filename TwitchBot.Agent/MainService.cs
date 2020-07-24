using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using TwitchBot.Agent.Services;
using TwitchBot.Agent.Services.Interfaces;

namespace TwitchBot.Agent
{
    public class MainService : IHostedService
    {
        private readonly ITwitchChatBotService _twitchChatBot;

        public MainService(ITwitchChatBotService twitchChatBot)
        {
            _twitchChatBot = twitchChatBot;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _twitchChatBot.Connect();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _twitchChatBot.Disconnect();
            return Task.CompletedTask;
        }
    }
}