namespace Shared.Contracts
{
    public sealed record RecoveryPasswordRequest(string Login, string Verifier, string ClientSalt, string EncryptedDek, int CryptoVersion);
}