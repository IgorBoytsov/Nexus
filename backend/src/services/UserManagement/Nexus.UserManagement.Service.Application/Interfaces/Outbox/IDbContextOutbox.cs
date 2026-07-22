using Shared.Kernel.Primitives;

namespace Nexus.UserManagement.Service.Application.Interfaces.Outbox
{
    public interface IDbContextOutbox
    {
        void Append(IReadOnlyCollection<IDomainEvent> domainEvents);
    }
}