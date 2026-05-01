namespace Shared.Contracts
{
    public sealed record RecoveryPasswordRequest(string Login, string Verifier, string ClientSalt, string EncryptedDek, string EncryptionAlgorithm, int Iterations, string KdfType);
}