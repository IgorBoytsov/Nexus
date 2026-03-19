namespace Rebout.Nexus.Contracts.UserManagement.v1
{
    public record PublicEncryptionInfoResponse(string ClientSalt, string EncryptedDek);
}