namespace Shared.Contracts
{
    public record UserAuthDataResponse(
        string Id, 
        string Login, 
        string Verifier, 
        string ClientSalt, 
        int SrpVersion,
        string EncryptedVerifierWrapKey,
        int KeyWrapVersion,
        string AsymmetricKeyId,
        string EncryptedDek, 
        List<string> Roles);
}