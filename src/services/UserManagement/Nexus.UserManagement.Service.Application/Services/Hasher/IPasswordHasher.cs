namespace Nexus.UserManagement.Service.Application.Services.Hasher
{
    public interface IPasswordHasher
    {
        string HashPassword(string password);
        bool VerifyPassword(string password, string storedHashString);
        CryptoParameter GetParametersFromHash(string storedHashString);
    }
}