namespace Shared.Contracts.UserManagement.Responses
{
    public record UserAuthDataResponse(
        string Id, 
        string Login, 
        string EncryptedDek, 
        int DekVersion,
        string EncryptedVerifier, 
        string ClientSalt, 
        int SrpVersion,
        int SrpCryptoVersion,
        string EncryptedVerifierWrapKey,
        int KeyWrapVersion,
        string AsymmetricKeyId,
        List<string> Roles);
}