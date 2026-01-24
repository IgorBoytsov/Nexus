namespace Shared.Contracts.UserMenagement.Responses
{
    public sealed record PublicEncryptionInfoResponse(string ClientSalt, string EncryptedDek);
}