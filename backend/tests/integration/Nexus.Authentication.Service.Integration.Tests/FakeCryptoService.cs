using Crossdyne.Security.Abstractions;
using Crossdyne.Security.Configuration;

namespace Nexus.Authentication.Service.Integration.Tests
{
    public class FakeCryptoService : ICryptoService
    {
        public T? DecryptData<T>(string encryptedData, byte[] key)
        {
            if (typeof(T) == typeof(string))
                return (T?)(object)Convert.ToBase64String(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 });
                
            return default;
        }

        public string EncryptData<T>(T data, byte[] key, CryptoVersion version = CryptoVersion.V1)
        {
            throw new NotImplementedException();
        }

        public byte[] GenerateRandomBytes(int length = 32)
        {
            throw new NotImplementedException();
        }
    }
}