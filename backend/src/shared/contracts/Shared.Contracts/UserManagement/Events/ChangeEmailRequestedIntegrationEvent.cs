using Shared.Contracts.Messaging.Interfaces;

namespace Shared.Contracts.UserManagement.Events
{
    public sealed record ChangeEmailRequestedIntegrationEvent(
        string IdEvent, 
        string OccurredOnUtc, 
        string UserId, 
        string To, 
        string Subject, 
        string Body,
        string ExpiresAt) : IIntegrationEvent;
}