namespace Shared.Contracts.UserMenagement.Responses
{
    public sealed record UserAuthDataResponse(Guid Id, string Login, string Verifier, string ClientSalt, string EncryptedDek, List<string> Roles);
}