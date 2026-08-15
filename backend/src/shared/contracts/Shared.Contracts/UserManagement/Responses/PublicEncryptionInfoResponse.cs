namespace Shared.Contracts.UserManagement.Responses
{
    public record PublicEncryptionInfoResponse(string ClientSalt, string EncryptedDek);
}