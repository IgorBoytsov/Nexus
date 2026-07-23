using Nexus.UserManagement.Service.Domain.ValueObjects.User;
using Shared.Kernel.Primitives;

namespace Nexus.UserManagement.Service.Domain.Events
{
    public sealed record UserAccountDeletedDomainEvent(Guid IdEvent, DateTime OccurredOnUtc, UserId UserId) : IDomainEvent;
}