using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;


using TwitchBot.Common.Data;



namespace TwitchBot.Agent
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            IConfiguration Configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .AddUserSecrets<Program>()
                .AddCommandLine(args)
                .Build();

            IServiceCollection services = new ServiceCollection();

            services
                .AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")))

                //.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                    //.AddEntityFrameworkStores<ApplicationDbContext>()



                //.Configure<LuisConfiguration>(Configuration.GetSection(nameof(LuisConfiguration)))
                //.Configure<TwitchConfiguration>(Configuration.GetSection(nameof(TwitchConfiguration)))
                //.Configure<ChatConfiguration>(Configuration.GetSection(nameof(ChatConfiguration)))
                //.Configure<LuisChatResponses>(Configuration.GetSection(nameof(LuisChatResponses)))
                //.Configure<CosmosDbConfiguration>(Configuration.GetSection(nameof(CosmosDbConfiguration)))

                .AddOptions()

                //.AddSingleton<ITwitchChatBotService, TwitchChatBotService>()
                //.AddSingleton<ILuisService, LuisService>()
                //.AddSingleton<ICosmosDbService, CosmosDbService>()
                .BuildServiceProvider();

            var serviceProvider = services.BuildServiceProvider();

            //ICosmosDbService cosmosDbService  = serviceProvider.GetService<ICosmosDbService>();
            //await cosmosDbService.StartupDatabase();


            // --------------- test cosmos directly --------------------
            //ICosmosDbService cosmosDbService  = serviceProvider.GetService<ICosmosDbService>();
            //await cosmosDbService.DoStuff();
            //Console.ReadLine();
            // --------------- test cosmos directly --------------------



            //var luisHandler = serviceProvider.GetService<ILuisService>();
            //IntentResponse intentResponse = await luisHandler.GetIntent("i found this interesting website blogs.siliconorchid.com");
            //Console.WriteLine($"{intentResponse.Intent} - {intentResponse.Certainty}");
            //Console.ReadLine();


            ////connect chatbot -------------------------------------------------------------------
            //ITwitchChatBotService twitchChatBot = serviceProvider.GetService<ITwitchChatBotService>();
            //twitchChatBot.Connect();
            //Console.ReadLine();
            //twitchChatBot.Disconnect();
            ////end connect chatbot------------------------------------------------------------------ -
        }
    }
}
