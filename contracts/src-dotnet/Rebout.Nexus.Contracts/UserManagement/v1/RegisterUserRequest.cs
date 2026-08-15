namespace Rebout.Nexus.Contracts.UserManagement.v1
{
    public record RegisterUserRequest(string Login, string UserName, string Verifier, string ClientSalt, string EncryptedDek, string EncryptionAlgorithm, int Iterations, string KdfType, string Email, string? Phone, string? IdGender, string? IdCountry);
}