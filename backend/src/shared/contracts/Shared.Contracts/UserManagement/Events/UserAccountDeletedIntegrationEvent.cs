using Shared.Abstractions.Messaging;

namespace Shared.Contracts.UserManagement.Events
{
    public sealed record UserAccountDeletedIntegrationEvent(
        Guid IdEvent, 
        DateTime OccurredOnUtc, 
        Guid UserId) : IIntegrationEvent;
}