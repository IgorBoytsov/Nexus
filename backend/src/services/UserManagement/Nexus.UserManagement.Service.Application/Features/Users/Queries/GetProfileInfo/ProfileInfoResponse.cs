namespace Nexus.UserManagement.Service.Application.Features.Users.Queries.GetProfileInfo
{
    public sealed record ProfileInfoResponse(string Login, string Email, string? PhonNumber);
}