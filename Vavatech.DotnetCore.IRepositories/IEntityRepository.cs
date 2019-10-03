using System.Collections.Generic;
using Vavatech.DotnetCore.Models;

namespace Vavatech.DotnetCore.IRepositories
{
    public interface IEntityRepository<TEntity, TKey>
        where TEntity : IEntity<TKey>
    {
        IEnumerable<TEntity> Get();
        TEntity Get(TKey id);
        void Add(TEntity entity);
        void Update(TEntity entity);
        void Remove(TKey id);
        bool Exists(TKey id);
    }

}
