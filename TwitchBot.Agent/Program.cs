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
using TwitchBot.Agent.Services;
using TwitchBot.Agent.Services.Interfaces;
using TwitchBot.Common.Data;
using TwitchBot.Common.Data.Repository.Interfaces;
using TwitchBot.Common.Models.Chat;
using TwitchBot.Common.Models.Configuration;
using TwitchBot.Common.Models.Entity;
using TwitchBot.Common.StartupExtensions;

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







            //.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
            //.AddEntityFrameworkStores<ApplicationDbContext>()

            services.Configure<FeatureSwitchesConfig>(Configuration.GetSection(nameof(FeatureSwitchesConfig) ));
            services.Configure<MemoryCachingConfig>(Configuration.GetSection(nameof(MemoryCachingConfig)));

            services.Configure<LuisConfiguration>(Configuration.GetSection(nameof(LuisConfiguration)));
            services.Configure<TwitchConfiguration>(Configuration.GetSection(nameof(TwitchConfiguration)));
            services.Configure<ChatConfiguration>(Configuration.GetSection(nameof(ChatConfiguration)));
            services.Configure<LuisChatResponses>(Configuration.GetSection(nameof(LuisChatResponses)));
            //.Configure<CosmosDbConfiguration>(Configuration.GetSection(nameof(CosmosDbConfiguration)))

            services.AddOptions();
            services.AddMemoryCache();


            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                Configuration.GetConnectionString("DefaultConnection")));



            services.AddSingleton<ITwitchChatBotService, TwitchChatBotService>();
            services.AddSingleton<ILuisService, LuisService>();
            services.AddSingleton<IChatMessageService, ChatMessageService>();
            services.AddCommonServices(); // defined in "common" projects



            //.AddSingleton<ICosmosDbService, CosmosDbService>()
            //services.BuildServiceProvider();

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


            //connect chatbot -------------------------------------------------------------------
            ITwitchChatBotService twitchChatBot = serviceProvider.GetService<ITwitchChatBotService>();
            twitchChatBot.Connect();
            Console.ReadLine();
            twitchChatBot.Disconnect();
            //end connect chatbot------------------------------------------------------------------ -
            ///

            //var twitchUser = new TwitchUserChat { TwitchUserName = "mrblobby2" };

            //var twitchUserRepository = serviceProvider.GetService<ITwitchUserChatRepository>();

            //await twitchUserRepository.CreateAsync(twitchUser);



        }
    }
}
