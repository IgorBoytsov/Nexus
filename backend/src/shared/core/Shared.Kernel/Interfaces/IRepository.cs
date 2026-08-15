using System.Linq.Expressions;
using Crossdyne.Toolkit.Primitives;
using Shared.Kernel.Primitives;

namespace Shared.Kernel.Interfaces
{
    public interface IRepository<TEntity> where TEntity : class, IAggregateRoot
    {
        Task<TEntity> AddAsync(TEntity entity, CancellationToken cl = default);
        Task<IEnumerable<TEntity>> AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken clt = default);
        Task<IEnumerable<TEntity>> AddRangeAsync(CancellationToken clt = default, params TEntity[] entities);
        void Remove(TEntity entity);
        void RemoveRange(params TEntity[] entities);
        void RemoveRange(IEnumerable<TEntity> entities);

        Task<Maybe<TEntity>> GetByAsync(Expression<Func<TEntity, bool>> expression, CancellationToken clt = default, params Expression<Func<TEntity, object>>[] includes);

        Task<bool> Exist(Expression<Func<TEntity, bool>> expression, CancellationToken clt = default);
    }
}