using System.Collections.Generic;
using System.Threading.Tasks;
using Vavatech.DotnetCore.Models;

namespace Vavatech.DotnetCore.IRepositories
{
    public interface IEntityRepositoryAsync<TEntity, TKey>
       where TEntity : IEntity<TKey>
    {
        Task<IEnumerable<TEntity>> GetAsync();
        Task<TEntity> GetAsync(TKey id);
        Task AddAsync(TEntity entity);
        Task UpdateAsync(TEntity entity);
        Task RemoveAsync(TKey id);
    }

}
