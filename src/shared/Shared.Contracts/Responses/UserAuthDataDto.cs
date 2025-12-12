namespace Shared.Contracts.Responses
{
    public sealed record UserAuthDataDto(Guid Id, string Login, string PasswordHash, string ClientSalt, string EncryptedDek, List<string> Roles);
}