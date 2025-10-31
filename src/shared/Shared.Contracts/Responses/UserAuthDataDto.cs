namespace Shared.Contracts.Responses
{
    public sealed record UserAuthDataDto(Guid Id, string Login, string PasswordHash, List<string> Roles);
}