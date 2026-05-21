namespace Shared.Contracts
{
    public sealed record RecoveryViaKeysPayloadResponse(List<RecoveryKeysResponse> RecoveryKeys);

    public sealed record RecoveryKeysResponse(string Key, int CryptoVersion); 
}