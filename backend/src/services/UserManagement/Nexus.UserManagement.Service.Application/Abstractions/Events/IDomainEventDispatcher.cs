using Shared.Kernel.Primitives;

namespace Nexus.UserManagement.Service.Application.Abstractions.Events
{
    public interface IDomainEventDispatcher
    {
        Task DispatchAsync(IReadOnlyCollection<IDomainEvent> events, CancellationToken ct = default);
    }
}