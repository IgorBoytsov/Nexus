namespace Shared.Contracts.UserMenagement.Requests
{
    public sealed record RecoveryAccessRequest(string Login, string Email, string NewPassword);
}