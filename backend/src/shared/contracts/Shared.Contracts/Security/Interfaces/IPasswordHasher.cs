using Shared.Contracts.Security.Models;

namespace Shared.Contracts.Security.Interfaces
{
    public interface IPasswordHasher
    {
        string HashPassword(string password);
        bool VerifyPassword(string password, string storedHashString);
        CryptoParameter GetParametersFromHash(string storedHashString);
    }
}