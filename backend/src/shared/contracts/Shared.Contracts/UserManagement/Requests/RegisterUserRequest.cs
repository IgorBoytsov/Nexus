namespace Shared.Contracts.UserManagement.Requests
{
    public record RegisterUserRequest(
        // Общая информация об аккаунте
        string Login, 
        string UserName, 
        string Email, 
        string? IdGender, 
        string? IdCountry,
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
        int CryptoVersion,
        // RecoveryKeys
        IReadOnlyCollection<RecoveryKeyData> RecoveryKeys);

    public record RecoveryKeyData(string EncryptedValue, int CryptoVersion);
}