namespace Shared.Contracts
{
    public record RegisterUserRequest(
        string Login, string UserName, 
        string Verifier, int SrpVersion, string ClientSalt, 
        string EncryptedVerifierWrapKey, int CryptoVersion,
        string EncryptedDek, int KeyWrapVersion, string AsymmetricKeyId, 
        string Email, string? IdGender, string? IdCountry,
        IReadOnlyCollection<RecoveryKeyData> RecoveryKeys);

    public record RecoveryKeyData(string EncryptedValue, int CryptoVersion);
}