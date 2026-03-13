using Konscious.Security.Cryptography;
using System.Security.Cryptography;
using System.Text;

namespace Shared.Security.Hasher
{
    public class Argon2PasswordHasher : IPasswordHasher
    {
        private const int ArgonDegreeOfParallelism = 2;
        private const int ArgonIterations = 3;
        private const int ArgonMemorySizeKb = 65536;

        private const int SaltSize = 16;
        private const int HashSize = 32;

        public string HashPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
                throw new ArgumentNullException(nameof(password));

            byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);

            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

            var argon2 = new Argon2id(passwordBytes)
            {
                Salt = salt,
                DegreeOfParallelism = ArgonDegreeOfParallelism,
                Iterations = ArgonIterations,
                MemorySize = ArgonMemorySizeKb
            };

            byte[] hash = argon2.GetBytes(HashSize);

            string saltBase64 = Convert.ToBase64String(salt);
            string hashBase64 = Convert.ToBase64String(hash);

            return $"Argon2id:{ArgonDegreeOfParallelism}:{ArgonIterations}:{ArgonMemorySizeKb}:{saltBase64}:{hashBase64}";
        }

        public bool VerifyPassword(string password, string storedHashString)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(storedHashString))
                return false;

            var parseResult = ParseHashString(storedHashString);

            if (!parseResult.Success)
                return false;

            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

            var argon2 = new Argon2id(passwordBytes)
            {
                Salt = parseResult.Parameters.Salt,
                DegreeOfParallelism = parseResult.Parameters.DegreeOfParallelism,
                Iterations = parseResult.Parameters.Iterations,
                MemorySize = parseResult.Parameters.MemorySizeKb
            };

            byte[] testHash = argon2.GetBytes(parseResult.StoredHash.Length);

            bool verified = CryptographicOperations.FixedTimeEquals(testHash, parseResult.StoredHash);

            return verified;
        }

        public CryptoParameter GetParametersFromHash(string storedHashString)
        {
            var parseResult = ParseHashString(storedHashString);
            if (!parseResult.Success)
                return null!;

            return parseResult.Parameters;
        }

        private (bool Success, CryptoParameter Parameters, byte[] StoredHash) ParseHashString(string storedHashString)
        {
            if (string.IsNullOrEmpty(storedHashString))
                return (false, null!, null!);

            string[] parts = storedHashString.Split(':');

            if (parts.Length != 6 || parts[0] != "Argon2id")
                return (false, null!, null!);

            try
            {
                var parameters = new CryptoParameter
                {
                    DegreeOfParallelism = int.Parse(parts[1]),
                    Iterations = int.Parse(parts[2]),
                    MemorySizeKb = int.Parse(parts[3]),
                    Salt = Convert.FromBase64String(parts[4])
                };

                byte[] storedHash = Convert.FromBase64String(parts[5]);

                return (true, parameters, storedHash);
            }
            catch (Exception)
            {
                return (false, null!, null!);
            }
        }
    }
}
