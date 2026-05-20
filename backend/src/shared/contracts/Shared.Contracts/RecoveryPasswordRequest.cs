namespace Shared.Contracts
{
    public sealed record RecoveryPasswordRequest(
        string Login, 
        string Verifier, 
        string ClientSalt, 
        string EncryptedDek, 
        int CryptoVersion, 
        int SrpVersion, 
        string EncryptedVerifierWrapKey, 
        int KeyWrapVersion, 
        string AsymmetricKeyId,
        IReadOnlyCollection<RecoveryKeysRequestData> RecoveryKeys);

    public sealed record RecoveryKeysRequestData(string EncryptedValue, int CryptoVersion); 
}