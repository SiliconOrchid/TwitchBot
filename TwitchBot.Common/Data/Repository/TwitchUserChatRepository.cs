
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

using TwitchBot.Common.Data.Repository.Interfaces;
using TwitchBot.Common.Models.Configuration;
using TwitchBot.Common.Models.Entity;

namespace TwitchBot.Common.Data.Repository
{


    public class TwitchUserChatRepository : GenericRepository<TwitchUserChat>, ITwitchUserChatRepository
    {


        public TwitchUserChatRepository(
            ApplicationDbContext applicationDbContext,
            IMemoryCache memoryCache,
            IOptions<FeatureSwitchesConfig> featureSwitchesConfig,
            IOptions<MemoryCachingConfig> memoryCachingConfig
        )
        : base(applicationDbContext, memoryCache, featureSwitchesConfig, memoryCachingConfig)
        {

        }

        public async Task<IEnumerable<TwitchUserChat>> GetAllOrderedAsync()
        {
            // default to returning an empty collection rather than null
            IEnumerable<TwitchUserChat> items = new List<TwitchUserChat>();

            try
            {
                string cacheKey = $"SampleEntityRepository_GetAllOrderedAsync";

                if (base._featureSwitchesConfig.EnableRepositoryMemoryCache && _memoryCache.TryGetValue(cacheKey, out items))
                {
                    // returned cached version
                    return items;
                }

                // no cached version, so query DB and set to cache
                items = await base.GetAll().OrderBy(x => x.TwitchUserDisplayName).ToListAsync();

                _memoryCache.Set<IEnumerable<TwitchUserChat>>(cacheKey, items, DateTime.Now.AddSeconds(_memoryCachingConfig.ShortCacheDurationSeconds));

                if (!items.Any())
                {
                    //Log.Warn($"{this.GetCallingClassAndMethod()} No SampleEntity records found");
                }

                return items;
            }
            catch (Exception ex)
            {
                //Log.Error($"{this.GetCallingClassAndMethod()} Unexpected exception : ", ex);
                throw;
            }

        }
    }
}