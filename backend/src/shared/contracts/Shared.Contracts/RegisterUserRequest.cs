namespace Shared.Contracts
{
    public record RegisterUserRequest(string Login, string UserName, string Verifier, string ClientSalt, string EncryptedDek, int CryptoVersion, string Email, string? IdGender, string? IdCountry);
}