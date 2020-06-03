using Microsoft.EntityFrameworkCore;
using TwitchBot.Common.Models.Entity;

namespace TwitchBot.Common.Data.EntityConfigurations
{
    public class TwitchUserEntityConfiguration
    {
        public static void Configure(ModelBuilder modelBuilder)
        {
            // HELP : EF Core reference for fluentAPI:  http://ef.readthedocs.io/en/latest/modeling/keys.html
            // HELP : FluentAPI reference:  http://ef.readthedocs.io/en/latest/modeling/relationships.html

            // TIP : ENSURE THAT this configuration is registered in "Context/IMCIntegrationDBContext/OnModelCreating"
            // TIP : EF Configuration is partly done in the "BaseEntity" (which is purpose of 'BaseEntityConfiguration.SetUp ...' below) - be aware that fields like the primary key and other common fields are configured there and not in this file.

            modelBuilder.Entity<TwitchUser>(b =>
            {
                modelBuilder = BaseEntityConfiguration.SetUp<TwitchUser>(modelBuilder, "TwitchUser", "TwitchBot");


                #region ---- Entity Main Properties  ----   
                b.Property(c => c.TwitchUserName)
                    .IsRequired()
                    .HasMaxLength(100);
                #endregion

                #region ---- Entity Indicies  ----     
                #endregion
            });

        }
    }
}
