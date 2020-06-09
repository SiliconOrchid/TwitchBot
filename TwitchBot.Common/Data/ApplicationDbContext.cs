using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using TwitchBot.Common.Data.EntityConfigurations;
using TwitchBot.Common.Data.Extensions;
using TwitchBot.Common.Models.Entity;

namespace TwitchBot.Common.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<TwitchUserChat> TwitchUserChats { get; set; }

        public DbSet<TwitchNewSubscriber> TwitchNewSubscribers { get; set; }

        public DbSet<TwitchUserAttendanceEvent> TwitchUserAttendanceEvents { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }





        /// <summary>
        /// Used as part of FluentAPI to allow us to annotate POCO models with database specific attributes
        /// Each entity has its own configuration - we register each of those items in this class.
        /// Registrations can be found in the "EntityConfiguration" namespace of this assembly.
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            TwitchUserChatEntityConfiguration.Configure(modelBuilder);
            TwitchNewSubscriberEntityConfiguration.Configure(modelBuilder);
            TwitchUserAttendanceEventEntityConfiguration.Configure(modelBuilder);
        }

        /// <summary>
        /// Allows us to globally run code, in this case updating the "Created" and "Modified" fields, without having to specify in each repo method.
        /// </summary>
        /// <param name="acceptAllChangesOnSuccess"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            ChangeTracker.ApplyAuditInformation();
            return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }
    }
}
