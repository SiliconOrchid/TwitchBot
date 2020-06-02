using System;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using TwitchBot.Service;
using TwitchBot.Models.Chat;
using TwitchBot.Models.Configuration;


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
                .Configure<LuisChatResponses>(Configuration.GetSection(nameof(LuisChatResponses)))
                .Configure<CosmosDbConfiguration>(Configuration.GetSection(nameof(CosmosDbConfiguration)))

                .AddOptions()

                .AddSingleton<ITwitchChatBotService, TwitchChatBotService>()
                .AddSingleton<ILuisService, LuisService>()
                .AddSingleton<ICosmosDbService, CosmosDbService>()
                .BuildServiceProvider();

            var serviceProvider = services.BuildServiceProvider();


            ICosmosDbService cosmosDbService  = serviceProvider.GetService<ICosmosDbService>();
            await cosmosDbService.DoStuff();
            Console.ReadLine();

            // var luisHandler = serviceProvider.GetService<ILuisHandler>(); 
            // IntentResponse intentResponse  = await luisHandler.GetIntent("what are we working on today");
            // Console.WriteLine($"{intentResponse.Intent} - {intentResponse.Certainty}");  
            //Console.ReadLine();


            //// connect chatbot -------------------------------------------------------------------
            //ITwitchChatBotService twitchChatBot = serviceProvider.GetService<ITwitchChatBotService>();    
            //twitchChatBot.Connect();
            //Console.ReadLine();
            //twitchChatBot.Disconnect();
            //// end connect chatbot -------------------------------------------------------------------
        }
    }
}


