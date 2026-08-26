namespace Nexus.UserManagement.Service.Application.Abstractions.Transactions
{
    public interface ITransactionManager
    {
        Task BeginAsync(CancellationToken ct = default);
        Task CommitAsync(CancellationToken ct = default);
        Task RollbackAsync(CancellationToken ct = default);
        bool HasActiveTransaction { get; }
    }
}