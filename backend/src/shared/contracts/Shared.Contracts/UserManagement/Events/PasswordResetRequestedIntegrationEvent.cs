using Shared.Abstractions.Messaging.Abstractions;

namespace Shared.Contracts.UserManagement.Events
{
    public sealed record PasswordResetRequestedIntegrationEvent(
        Guid IdEvent,
        DateTime OccurredOnUtc,
        Guid UserId,
        string To,
        string Subject,
        string Body,
        string ExpiresAt) : IIntegrationEvent;
}