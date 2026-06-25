namespace Shared.Contracts.UserManagement.Requests
{
    public sealed record ResetPasswordCompleteRequest(
        string Login, 
        // Srp
        string EncryptedVerifier, 
        string SrpSalt, 
        int SrpVersion, 
        string EncryptedVerifierWrapKey, 
        int KeyWrapVersion, 
        string AsymmetricKeyId,
        // Dek 
        string EncryptedDek,
        string DekSalt, 
        int CryptoVersion, 
        // RecoveryKeys
        IReadOnlyCollection<RecoveryKeysRequestData> RecoveryKeys);

    public sealed record RecoveryKeysRequestData(string EncryptedValue, int CryptoVersion); 
}