namespace Shared.Contracts.UserManagement.Requests
{
    public sealed record ChangePasswordRequest(
        string? UserId,
        // Srp
        string EncryptedVerifier, 
        string SrpSalt, 
        int SrpVersion, 
        int SrpCryptoVersion,
        string EncryptedVerifierWrapKey, 
        int KeyWrapVersion, 
        string AsymmetricKeyId,
        // Dek
        string EncryptedDek,
        string DekSalt,
        int CryptoVersion);
}