using Shared.Kernel.Primitives;

namespace Nexus.UserManagement.Service.Application.Abstractions.UnitOfWork
{
    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        IReadOnlyCollection<IDomainEvent> GetPendingDomainEvents();
        void ClearPendingDomainEvents();
        void Dispose();
    }
}