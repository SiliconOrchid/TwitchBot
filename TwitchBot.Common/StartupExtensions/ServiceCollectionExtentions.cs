using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using TwitchBot.Common.Data.Repository;
using TwitchBot.Common.Data.Repository.Interfaces;

namespace TwitchBot.Common.StartupExtensions
{
    public static class ServiceCollectionExtentions
    {
        public static IServiceCollection AddCommonServices(this IServiceCollection services)
        {
            RegisterCommonServices(services);
            return services;
        }

        private static void RegisterCommonServices(IServiceCollection services)
        {
            services.AddScoped<ITwitchUserChatRepository, TwitchUserChatRepository>();
            services.AddScoped<ITwitchNewSubscriberRepository, TwitchNewSubscriberRepository>();
            services.AddScoped<ITwitchUserAttendanceEventRepository, TwitchUserAttendanceEventRepository>();
        }
    }
}