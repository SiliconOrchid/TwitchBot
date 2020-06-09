using Microsoft.EntityFrameworkCore;
using TwitchBot.Common.Models.Entity;

namespace TwitchBot.Common.Data.EntityConfigurations
{
    public class TwitchUserChatEntityConfiguration
    {
        public static void Configure(ModelBuilder modelBuilder)
        {
            // HELP : EF Core reference for fluentAPI:  http://ef.readthedocs.io/en/latest/modeling/keys.html
            // HELP : FluentAPI reference:  http://ef.readthedocs.io/en/latest/modeling/relationships.html

            // TIP : ENSURE THAT this configuration is registered in "Context/IMCIntegrationDBContext/OnModelCreating"
            // TIP : EF Configuration is partly done in the "BaseEntity" (which is purpose of 'BaseEntityConfiguration.SetUp ...' below) - be aware that fields like the primary key and other common fields are configured there and not in this file.

            modelBuilder.Entity<TwitchUserChat>(b =>
            {
                modelBuilder = BaseEntityConfiguration.SetUp<TwitchUserChat>(modelBuilder, "TwitchUserChat", "TwitchBot");


                #region ---- Entity Main Properties  ----   
                b.Property(c => c.TwitchUserId)
                    .IsRequired()
                    .HasMaxLength(50);

                b.Property(c => c.TwitchUserType)
                    .HasMaxLength(50);

                b.Property(c => c.TwitchUserDisplayName)
                    .IsRequired()
                    .HasMaxLength(100);

                b.Property(c => c.ChatMessage)
                    .HasMaxLength(4096);

                #endregion

                #region ---- Entity Indicies  ----     
                #endregion
            });

        }
    }
}
