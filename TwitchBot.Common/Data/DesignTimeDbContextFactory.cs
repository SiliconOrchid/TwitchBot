using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TwitchBot.Common.Data
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        /// <summary>
        /// Required to support command line tooling (i.e. to be able to run "dotnet ef migrations add initial" etc)
        /// </summary>
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var devEnvironmentVariable = Environment.GetEnvironmentVariable("NETCORE_ENVIRONMENT");
            var isDevelopment = string.IsNullOrEmpty(devEnvironmentVariable) || devEnvironmentVariable.ToLower() == "development";


            DbContextOptionsBuilder<ApplicationDbContext> builder;
            IConfigurationRoot configuration;


            if (isDevelopment)
            {
                //only use UserSecrets (for connection string) in development
                configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    .AddUserSecrets<ApplicationDbContext>()
                    .Build();
                builder = new DbContextOptionsBuilder<ApplicationDbContext>();
            }
            else
            {
                configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    .Build();
                builder = new DbContextOptionsBuilder<ApplicationDbContext>();
            }

            var connectionString = configuration.GetConnectionString("DefaultConnection");
            builder.UseSqlServer(connectionString);

            return new ApplicationDbContext(builder.Options);
        }
    }
}
