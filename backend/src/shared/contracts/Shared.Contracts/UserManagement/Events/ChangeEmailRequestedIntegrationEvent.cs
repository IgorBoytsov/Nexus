using Shared.Abstractions.Messaging;

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