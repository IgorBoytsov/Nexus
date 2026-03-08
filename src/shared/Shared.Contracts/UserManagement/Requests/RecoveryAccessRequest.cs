namespace Shared.Contracts.UserManagement.Requests
{
    public sealed record RecoveryAccessRequest(string Login, string Email, string NewPassword);
}