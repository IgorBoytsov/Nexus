using Shared.Kernel.Primitives;

namespace Nexus.UserManagement.Service.Application.Abstractions.Outbox
{
    public interface IDbContextOutbox
    {
        void Append(IReadOnlyCollection<IDomainEvent> domainEvents);
    }
}