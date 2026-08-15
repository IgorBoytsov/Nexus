using System.Security.Cryptography;
using System.Text;

namespace Nexus.Authentication.Service.Application.Extensions
{
    public static class TokenHasher
    {
        public static string Hash(string value) 
            => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value)));
    }
}