namespace Shared.Contracts.UserManagement.Responses;

public sealed record DekResponse(string UserId, string EncryptedValue, int CryptoVersion, string Type, string UpdateAt);