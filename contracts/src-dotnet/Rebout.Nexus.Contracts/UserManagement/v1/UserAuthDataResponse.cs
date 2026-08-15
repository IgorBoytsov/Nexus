namespace Rebout.Nexus.Contracts.UserManagement.v1
{
    public record UserAuthDataResponse(string Id, string Login, string Verifier, string ClientSalt, string EncryptedDek, List<string> Roles);
}