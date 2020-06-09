using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

using TwitchBot.Common.Data.Repository.Interfaces;
using TwitchBot.Common.Extensions;
using TwitchBot.Common.Models.Configuration;
using TwitchBot.Common.Models.Entity;

namespace TwitchBot.Common.Data.Repository
{
    public abstract class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class, IEntityBase
    {
        protected readonly ApplicationDbContext _dbContext;
        protected readonly IMemoryCache _memoryCache;
        protected readonly FeatureSwitchesConfig _featureSwitchesConfig;
        protected readonly MemoryCachingConfig _memoryCachingConfig;


        public GenericRepository(
            ApplicationDbContext dbContext,
            IMemoryCache memoryCache,
            IOptions<FeatureSwitchesConfig> featureSwitchesConfig,
            IOptions<MemoryCachingConfig> memoryCachingConfig
            )
        {
            _dbContext = dbContext;
            _memoryCache = memoryCache;
            _featureSwitchesConfig = featureSwitchesConfig.Value;
            _memoryCachingConfig = memoryCachingConfig.Value;


            if ((_memoryCachingConfig.ShortCacheDurationSeconds < 1) || (_memoryCachingConfig.LongCacheDurationSeconds < 1))
            {
                throw new ArgumentException($"{this.GetCallingClassAndMethod()} MemoryCachingConfig-> CacheDurationSeconds does not seem to be set correctly - check configuration.");
            }
        }


        /// <summary>
        /// Notice that this method returns an IQueryable (as opposed to perhaps an IEnumerable),
        /// this allows us to be a bit more specific about what we want to return.
        /// Example of consumption:
        ///     List<SampleEntity> result = await _sampleEntityRepository.GetAll().ToListAsync();
        /// </summary>
        /// <returns></returns>
        public IQueryable<TEntity> GetAll()
        {

            try
            {
                return _dbContext.Set<TEntity>().AsNoTracking();
            }
            catch (Exception ex)
            {
                //JM17Jul18 : Arguably no need to wrap in try/catch, as IQuerable this does not get executed until it is called from strategy.
                string errorMessage = $"Problem retrieving collection of data from generic repository - with type '{typeof(TEntity).Name}' : ";
                //Log.Error($"{this.GetCallingClassAndMethod()}{errorMessage}", ex);
                throw;
            }
        }


        public async Task<TEntity> GetAsync(int id)
        {
            try
            {
                string cacheKey = $"GenericRepository_{typeof(TEntity).Name}_{id}";
                TEntity cachedValue;

                if (_featureSwitchesConfig.EnableRepositoryMemoryCache && _memoryCache.TryGetValue(cacheKey, out cachedValue))
                {
                    // returned cached version
                    return cachedValue;
                }

                // no cached version, so query DB and set to cache
                var result = await _dbContext.Set<TEntity>()
                    .AsNoTracking()
                    .FirstOrDefaultAsync(e => e.Id == id);

                if (_featureSwitchesConfig.EnableRepositoryMemoryCache)
                {
                    _memoryCache.Set<TEntity>(cacheKey, result, DateTime.Now.AddSeconds(_memoryCachingConfig.ShortCacheDurationSeconds));
                }

                return result;
            }
            catch (Exception ex)
            {
                string errorMessage = $"Problem retrieving data from generic repository - with type '{typeof(TEntity).Name}' : ";
                //Log.Error($"{this.GetCallingClassAndMethod()}{errorMessage}", ex);
                throw;
            }
        }




        public async Task CreateAsync(TEntity entity)
        {
            try
            {
                await _dbContext.Set<TEntity>().AddAsync(entity);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                string errorMessage = $"Problem creating record using generic repository - with type '{typeof(TEntity).Name}' : ";
                //Log.Error($"{this.GetCallingClassAndMethod()}{errorMessage}", ex);
                throw;
            }
        }

        public async Task UpdateAsync(int id, TEntity entity)
        {
            try
            {
                _dbContext.Set<TEntity>().Update(entity);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                string errorMessage = $"Problem updating record using generic repository - with type '{typeof(TEntity).Name}' : ";
                //Log.Error($"{this.GetCallingClassAndMethod()}{errorMessage}", ex);
                throw;
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                var entity = await GetAsync(id);
                _dbContext.Set<TEntity>().Remove(entity);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                string errorMessage = $"Problem deleting record using generic repository - with type '{typeof(TEntity).Name}' : ";
                //Log.Error($"{this.GetCallingClassAndMethod()}{errorMessage}", ex);
                throw;
            }
        }
    }
}