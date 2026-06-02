namespace Shared.Contracts.UserManagement.Responses
{
    public sealed record ChangePasswordInitResponse(
        string Login, 
        string EncryptedDek, 
        string DekSalt, 
        int CryptoVersionDek, 
        string AsymmetricKeyId,
        int SrvVersion);
}