namespace Shared.Contracts.UserManagement.Responses
{
    public sealed record GetChangePasswordDataResponse(
        string Login, 
        string EncryptedDek, 
        string DekSalt, 
        int CryptoVersionDek, 
        string AsymmetricKeyId,
        int SrvVersion);
}