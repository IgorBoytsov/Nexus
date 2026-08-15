namespace Nexus.Authentication.Service.Application.Extensions
{
    public static class RedisKeyExtensions
    {
        public static string SrpSession(string login) => $"crossdyne:srp:{login}";
        public static string DistributedLock(string refreshToken) => $"crossdyne:shared:lock:sessions:{refreshToken}";
    }
}