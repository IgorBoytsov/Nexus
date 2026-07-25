namespace Shared.Contracts.UserManagement.Events
{
    public sealed record UserPasswordResetIntegrationEvent(Guid IdEvent, DateTime OccurredOnUtc, Guid UserId);
}