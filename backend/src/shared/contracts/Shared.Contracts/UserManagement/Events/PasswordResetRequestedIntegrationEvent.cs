using Shared.Contracts.Messaging.Interfaces;

namespace Shared.Contracts.UserManagement.Events
{
    public sealed record PasswordResetRequestedIntegrationEvent(
        Guid IdEvent,
        string OccurredOnUtc,
        Guid UserId,
        string To,
        string Subject,
        string Body,
        string ExpiresAt) : IIntegrationEvent;
}