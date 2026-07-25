namespace Shared.Contracts.UserManagement.Requests
{
    public sealed record ChangeEmailRequest(string Email, string Code);
}