using Crossdyne.Toolkit.Primitives;

namespace Shared.Kernel.Interfaces
{
    public interface IReadOnlyRepository<TEntity> where TEntity : class
    {
        Task<Maybe<TEntity>> GetByIdAsync(string id, CancellationToken cl = default);
        Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cl = default);
    }
}