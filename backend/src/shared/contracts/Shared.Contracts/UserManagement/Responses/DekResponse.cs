namespace Shared.Contracts.UserManagement.Responses;

public sealed record DekResponse(string ClientSalt, string EncryptedDek, int CryptoVersion, string Login);