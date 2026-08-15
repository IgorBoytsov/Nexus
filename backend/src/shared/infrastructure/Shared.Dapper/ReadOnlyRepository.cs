using System.Data;
using Crossdyne.Toolkit.Primitives;
using Dapper;
using Shared.Kernel.Interfaces;

namespace Shared.Dapper
{
    public abstract class ReadOnlyRepository<TEntity>(IDbConnection connection, string tableName) : IReadOnlyRepository<TEntity>
        where TEntity : class
    {
        protected readonly IDbConnection _connection = connection;
        protected string TableName = tableName;

        public virtual async Task<Maybe<TEntity>> GetByIdAsync(string id, CancellationToken cl = default)
        {
            var entity = await _connection.QueryFirstOrDefaultAsync<TEntity>($"SELECT * FROM {TableName} WHERE Id = @Id");

            return Maybe<TEntity>.Some(entity);
        }
    
        public virtual async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cl = default)
            => await _connection.QueryAsync<TEntity>($"SELECT * FROM {TableName} ORDER BY id");
    }

}