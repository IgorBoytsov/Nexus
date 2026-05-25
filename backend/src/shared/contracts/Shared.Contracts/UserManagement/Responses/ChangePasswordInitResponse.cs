namespace Shared.Contracts.UserManagement.Responses
{
    public sealed record ChangePasswordInitResponse(string Login, string EncryptedDek, int CryptoVersionDek, string ClientSalt, string AsymmetricKeyId);
}