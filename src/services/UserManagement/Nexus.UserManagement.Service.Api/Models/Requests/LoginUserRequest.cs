namespace Nexus.UserManagement.Service.Api.Models.Requests
{
    public sealed record LoginUserRequest(string Password, string Login, string Email);
}