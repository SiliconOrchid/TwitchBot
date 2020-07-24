
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;


using TwitchBot.Common.Data;
using TwitchBot.Common.Models.Chat;
using TwitchBot.Common.Models.Configuration;
using TwitchBot.Common.StartupExtensions;
using TwitchBot.Agent.Rules;
using TwitchBot.Agent.Services;
using TwitchBot.Agent.Services.Interfaces;


namespace TwitchBot.Agent
{



    public class Program
    {
        public static async Task Main(string[] args)
        {
            await CreateHostBuilder(args)
                .RunConsoleAsync();
        }
        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            return new HostBuilder()
                .ConfigureAppConfiguration((hostingContext, config) => ConfigureAppConfiguration(hostingContext, config, args))
                .ConfigureServices(ConfigureService)
                .ConfigureLogging(ConfigureHosting);
        }

        private static void ConfigureAppConfiguration(HostBuilderContext hostingContext, IConfigurationBuilder config, string[] args)
        {
            var envName = hostingContext.HostingEnvironment.EnvironmentName;
            config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            config.AddJsonFile($"appsettings.{envName}.json", optional: true, reloadOnChange: true);
            config.AddUserSecrets<Program>();
            config.AddEnvironmentVariables();

            if (args != null)
            {
                config.AddCommandLine(args);
            }
        }

        private static void ConfigureService(HostBuilderContext hostContext, IServiceCollection services)
        {


            services.AddOptions();
            services.AddMemoryCache();

            services.Configure<TwitchConfiguration>(hostContext.Configuration.GetSection(nameof(TwitchConfiguration)));
            services.Configure<ChatConfiguration>(hostContext.Configuration.GetSection(nameof(ChatConfiguration)));
            services.Configure<LuisConfiguration>(hostContext.Configuration.GetSection(nameof(LuisConfiguration)));
            services.Configure<LuisChatResponses>(hostContext.Configuration.GetSection(nameof(LuisChatResponses)));
            services.Configure<FeatureSwitchesConfig>(hostContext.Configuration.GetSection(nameof(FeatureSwitchesConfig)));
            services.Configure<MemoryCachingConfig>(hostContext.Configuration.GetSection(nameof(MemoryCachingConfig)));




            services.AddSingleton<ITwitchChatBotService, TwitchChatBotService>();
            services.AddSingleton<ILuisService, LuisService>();
            services.AddSingleton<IChatMessageService, ChatMessageService>();
            services.AddCommonServices(); // data repository services, defined in "common" project

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                hostContext.Configuration.GetConnectionString("DefaultConnection")));


            services.Scan(scan => scan.FromAssemblyOf<IChatMessageRule>()
                .AddClasses(classes => classes.AssignableTo<IChatMessageRule>())
                .AsImplementedInterfaces()
                .WithSingletonLifetime()
                );


            services.AddHostedService<MainService>();

        }

        private static void ConfigureHosting(HostBuilderContext hostingContext, ILoggingBuilder logging)
        {
            logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
            logging.AddConsole();
            logging.SetMinimumLevel(LogLevel.Trace);
        }
    }
}





















//public class Program
//    {
//        public static async Task Main(string[] args)
//        {
//            IConfiguration Configuration = new ConfigurationBuilder()
//                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
//                .AddEnvironmentVariables()
//                .AddUserSecrets<Program>()
//                .AddCommandLine(args)
//                .Build();

//            IServiceCollection services = new ServiceCollection();







//            //.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
//            //.AddEntityFrameworkStores<ApplicationDbContext>()

//            services.Configure<FeatureSwitchesConfig>(Configuration.GetSection(nameof(FeatureSwitchesConfig) ));
//            services.Configure<MemoryCachingConfig>(Configuration.GetSection(nameof(MemoryCachingConfig)));

//            services.Configure<LuisConfiguration>(Configuration.GetSection(nameof(LuisConfiguration)));
//            services.Configure<TwitchConfiguration>(Configuration.GetSection(nameof(TwitchConfiguration)));
//            services.Configure<ChatConfiguration>(Configuration.GetSection(nameof(ChatConfiguration)));
//            services.Configure<LuisChatResponses>(Configuration.GetSection(nameof(LuisChatResponses)));
//            //.Configure<CosmosDbConfiguration>(Configuration.GetSection(nameof(CosmosDbConfiguration)))

//            services.AddOptions();
//            services.AddMemoryCache();


//            services.AddDbContext<ApplicationDbContext>(options =>
//                options.UseSqlServer(
//                Configuration.GetConnectionString("DefaultConnection")));



//            services.AddSingleton<ITwitchChatBotService, TwitchChatBotService>();
//            services.AddSingleton<ILuisService, LuisService>();
//            services.AddSingleton<IChatMessageService, ChatMessageService>();
//            services.AddCommonServices(); // defined in "common" projects



//            //.AddSingleton<ICosmosDbService, CosmosDbService>()
//            //services.BuildServiceProvider();

//            var serviceProvider = services.BuildServiceProvider();

//            //ICosmosDbService cosmosDbService  = serviceProvider.GetService<ICosmosDbService>();
//            //await cosmosDbService.StartupDatabase();


//            // --------------- test cosmos directly --------------------
//            //ICosmosDbService cosmosDbService  = serviceProvider.GetService<ICosmosDbService>();
//            //await cosmosDbService.DoStuff();
//            //Console.ReadLine();
//            // --------------- test cosmos directly --------------------



//            //var luisHandler = serviceProvider.GetService<ILuisService>();
//            //IntentResponse intentResponse = await luisHandler.GetIntent("i found this interesting website blogs.siliconorchid.com");
//            //Console.WriteLine($"{intentResponse.Intent} - {intentResponse.Certainty}");
//            //Console.ReadLine();


//            //connect chatbot -------------------------------------------------------------------
//            ITwitchChatBotService twitchChatBot = serviceProvider.GetService<ITwitchChatBotService>();
//            twitchChatBot.Connect();
//            Console.ReadLine();
//            twitchChatBot.Disconnect();
//            //end connect chatbot------------------------------------------------------------------ -
//            ///

//            //var twitchUser = new TwitchUserChat { TwitchUserName = "mrblobby2" };

//            //var twitchUserRepository = serviceProvider.GetService<ITwitchUserChatRepository>();

//            //await twitchUserRepository.CreateAsync(twitchUser);



//        }
//    }
//}
