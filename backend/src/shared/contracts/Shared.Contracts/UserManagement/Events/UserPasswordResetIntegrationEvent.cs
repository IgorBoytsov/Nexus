namespace Shared.Contracts.UserManagement.Events
{
    public sealed record UserPasswordResetIntegrationEvent(string IdEvent, string OccurredOnUtc, string UserId);
}