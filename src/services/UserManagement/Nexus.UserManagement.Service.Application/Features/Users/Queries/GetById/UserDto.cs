namespace Nexus.UserManagement.Service.Application.Features.Users.Queries.GetById
{
    public sealed record UserDto(
        Guid Id,
        string Login,
        string UserName,
        string Email,
        string? Phone,
        string StatusName,
        DateTime DateRegistration,
        DateTime? DateEntry);
}