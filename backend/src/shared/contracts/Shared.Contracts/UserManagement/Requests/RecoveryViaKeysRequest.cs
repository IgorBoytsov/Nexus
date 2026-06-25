namespace Shared.Contracts.UserManagement.Requests
{
    public sealed record RecoveryViaKeysRequest(
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
        // Recovery Keys
        List<RecoveryKeyRequestData> RecoveryKeys);

        public record RecoveryKeyRequestData(string EncryptedValue, int CryptoVersion);
}