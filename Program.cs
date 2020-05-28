using System;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using TwitchBot.Service;
using TwitchBot.Models;


namespace TwitchBot
{
    class Program
    {
        static async Task Main(string[] args)
        {
            IConfiguration Configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .AddUserSecrets<Program>()
                .AddCommandLine(args)
                .Build();

            IServiceCollection services = new ServiceCollection();

            services
                .Configure<LuisConfiguration>(Configuration.GetSection(nameof(LuisConfiguration)))
                .Configure<TwitchConfiguration>(Configuration.GetSection(nameof(TwitchConfiguration)))
                .Configure<ChatConfiguration>(Configuration.GetSection(nameof(ChatConfiguration)))
                .AddOptions()
                //.AddLogging()
                //.AddSingleton<ISecretRevealer, SecretRevealer>()
                .AddSingleton<ITwitchChatBot, TwitchChatBot>()
                .AddSingleton<ILuisHandler, LuisHandler>()
                .BuildServiceProvider();

            var serviceProvider = services.BuildServiceProvider();

            var luisHandler = serviceProvider.GetService<ILuisHandler>(); //todo: shouldn't this be an ILuisHandler?

            var jsonResponse = await luisHandler.MakeRequest("what are you doing?");

            var parseResponse = luisHandler.ParseResponse(jsonResponse);

            Console.WriteLine($"{jsonResponse}");    
            Console.WriteLine($"{parseResponse.Item1} - {parseResponse.Item2}");  


            Console.ReadLine();

            // TwitchChatBot twitchChatBot = serviceProvider.GetService<ITwitchChatBot>();    
            // twitchChatBot.Connect();


            // Console.ReadLine();
            // twitchChatBot.Disconnect();
        }
    }
}


