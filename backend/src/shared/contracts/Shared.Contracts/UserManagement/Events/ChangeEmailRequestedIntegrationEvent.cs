using Shared.Contracts.Messaging.Abstractions;

namespace Shared.Contracts.UserManagement.Events
{
    public sealed record ChangeEmailRequestedIntegrationEvent(
        Guid IdEvent, 
        DateTime OccurredOnUtc, 
        string UserId, 
        string To, 
        string Subject, 
        string Body,
        string ExpiresAt) : IIntegrationEvent;
}