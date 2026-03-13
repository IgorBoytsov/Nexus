namespace Shared.Security.Hasher
{
    public interface IPasswordHasher
    {
        string HashPassword(string password);
        bool VerifyPassword(string password, string storedHashString);
        CryptoParameter GetParametersFromHash(string storedHashString);
    }
}