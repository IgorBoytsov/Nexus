using Microsoft.Extensions.Configuration;
using Shared.Security.Verifiers;
using System.Security.Cryptography;

namespace Nexus.Authentication.Service.Application.Secure
{
    public sealed class RsaDecryptor(IConfiguration configuration) : IVerifierProtector
    {
        private readonly string _privateKeyBase64 = configuration["Crypto:RsaPrivateKey"]!;

        /// <exception cref="NotSupportedException"></exception>
        public string Protect(string verifier)
                => throw new NotSupportedException("Данная реализация не умеет шифровать.");

        public string Unprotect(string protectedVerifier)
        {
            using var rsa = RSA.Create();
            rsa.ImportRSAPrivateKey(Convert.FromBase64String(_privateKeyBase64), out _);

            var encryptedData = Convert.FromBase64String(protectedVerifier);
            var decryptedBytes = rsa.Decrypt(encryptedData, RSAEncryptionPadding.OaepSHA256);

            return Convert.ToBase64String(decryptedBytes);
        }
    }
}