namespace Shared.Contracts.UserManagement.Requests
{
    public sealed record ChangePasswordRequest(
        string? UserId,
        string Verifier, 
        string ClientSalt, 
        string EncryptedDek,
        int CryptoVersion, 
        int SrpVersion, 
        string EncryptedVerifierWrapKey, 
        int KeyWrapVersion, 
        string AsymmetricKeyId);
}