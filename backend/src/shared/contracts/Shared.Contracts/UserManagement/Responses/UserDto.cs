namespace Shared.Contracts.UserManagement.Responses
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