namespace Shared.Contracts
{
    public sealed record RecoveryViaKeysSetRequest(
        string Login,
        string Verifier,
        string ClientSalt,
        string EncryptedVerifierWrapKey,
        int CryptoVersion,
        int SrpVersion,
        string EncryptedDek, 
        int KeyWrapVersion,
        string AsymmetricKeyId,
        List<RecoveryKeyRequestData> RecoveryKeys);

        public record RecoveryKeyRequestData(string EncryptedValue, int CryptoVersion);
}