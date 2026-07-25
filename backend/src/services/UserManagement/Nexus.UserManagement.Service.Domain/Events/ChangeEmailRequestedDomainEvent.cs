using Nexus.UserManagement.Service.Domain.ValueObjects.User;
using Shared.Kernel.Primitives;

namespace Nexus.UserManagement.Service.Domain.Events
{
    public sealed record ChangeEmailRequestedDomainEvent(Guid IdEvent, DateTime OccurredOnUtc, UserId UserId, Email Email, string Code, DateTime ExpiresAt) : IDomainEvent;
}