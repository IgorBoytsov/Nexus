

using Shared.Abstractions.Security;

namespace Nexus.Authentication.Service.Integration.Tests
{
    public class FakeDataProtector : IDataProtector
    {
        public string Protect(string data)
        {
            throw new NotImplementedException();
        }

        public string Unprotect(string protectedData)
            => Convert.ToBase64String(new byte[] { 1, 2, 3, 4, 5 });
    }
}