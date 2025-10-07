namespace Nexus.UserManagement.Service.Api.Models.Requests
{
    public sealed record RecoveryAccessRequest(string Login, string Email, string NewPassword);
}