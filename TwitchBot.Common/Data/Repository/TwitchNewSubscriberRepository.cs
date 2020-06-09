
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
    public class TwitchNewSubscriberRepository : GenericRepository<TwitchNewSubscriber>, ITwitchNewSubscriberRepository
    {


        public TwitchNewSubscriberRepository(
            ApplicationDbContext applicationDbContext,
            IMemoryCache memoryCache,
            IOptions<FeatureSwitchesConfig> featureSwitchesConfig,
            IOptions<MemoryCachingConfig> memoryCachingConfig
        )
        : base(applicationDbContext, memoryCache, featureSwitchesConfig, memoryCachingConfig)
        {

        }

    }
}