using System.Linq;
using System.Threading.Tasks;

using TwitchBot.Common.Models.Entity;

namespace TwitchBot.Common.Data.Repository.Interfaces
{
    public interface IGenericRepository<TEntity> where TEntity : class, IEntityBase
    {
        IQueryable<TEntity> GetAll();
        Task<TEntity> GetAsync(int id);
        Task CreateAsync(TEntity entity);
        Task UpdateAsync(int id, TEntity entity);
        Task DeleteAsync(int id);
    }
}