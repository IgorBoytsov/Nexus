namespace Shared.Contracts.UserMenagement.Requests
{
    public sealed record class RegisterUserRequest(
        string Login, 
        string UserName,
        string Verifier, 
        string ClientSalt,
        string EncryptedDek,
        string EncryptionAlgorithm,
        int Iterations,
        string KdfType,
        string Email, string? Phone,
        Guid? IdGender, Guid? IdCountry);
}