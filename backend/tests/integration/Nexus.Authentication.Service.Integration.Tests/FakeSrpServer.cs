using Crossdyne.Security.Abstractions;
using Crossdyne.Security.Configuration;

namespace Nexus.Authentication.Service.Integration.Tests
{
    public class FakeSrpServer : ISrpServer
    {
           public SrpSessionState GetSrpChallenge(string login, byte[] v, byte[] salt, SrpGroup group)
            => new(login, [4, 5, 6], v, [7, 8, 9], salt);

        public string VerifySrpProof(SrpSessionState session, string A, string M1, SrpGroup group)
            => Convert.ToBase64String(new byte[] { 9, 8, 7, 6 });
    } 
}