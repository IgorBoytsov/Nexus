namespace Shared.Contracts.UserManagement.Responses
{
    public sealed record PublicEncryptionInfoResponse(string ClientSalt, string EncryptedDek);
}