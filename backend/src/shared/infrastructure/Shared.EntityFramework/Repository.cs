using System.Linq.Expressions;
using Crossdyne.Toolkit.Primitives;
using Microsoft.EntityFrameworkCore;
using Shared.Kernel.Interfaces;
using Shared.Kernel.Primitives;

namespace Shared.EntityFramework
{
    public abstract class Repository<TEntity, TContext> : IRepository<TEntity>
        where TEntity : class, IAggregateRoot
        where TContext : DbContext
    {
        protected readonly TContext _context;
        protected readonly DbSet<TEntity> _entity;

        protected Repository(TContext context)
        {
            _context = context;
            _entity = _context.Set<TEntity>();
        }

        public virtual async Task<TEntity> AddAsync(TEntity entity, CancellationToken clt = default)
        {
            await _entity.AddAsync(entity, clt);
            return entity;
        }

        public virtual async Task<IEnumerable<TEntity>> AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken clt = default)
        {
            await _entity.AddRangeAsync(entities, clt);
            return entities;
        }

        public virtual async Task<IEnumerable<TEntity>> AddRangeAsync(CancellationToken clt = default, params TEntity[] entities)
        {
            await _entity.AddRangeAsync(entities, clt);
            return entities;
        }

        public virtual void Remove(TEntity entity) => _entity.Remove(entity);
        public virtual void RemoveRange(params TEntity[] entities) => _entity.RemoveRange(entities);
        public virtual void RemoveRange(IEnumerable<TEntity> entities) => _entity.RemoveRange(entities);

        public async virtual Task<Maybe<TEntity>> GetByAsync(Expression<Func<TEntity, bool>> expression, CancellationToken clt = default, params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> query = _entity;

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            var result = await query.FirstOrDefaultAsync(expression, clt);

            return result is null 
                    ? Maybe<TEntity>.None 
                    : Maybe<TEntity>.Some(result);
        }

        public async virtual Task<bool> Exist(Expression<Func<TEntity, bool>> expression, CancellationToken clt = default) => await _entity.AnyAsync(expression, clt);
    }
}