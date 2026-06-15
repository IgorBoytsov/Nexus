namespace Nexus.Authentication.Service.Application.Extensions
{
    public static class RedisKeyExtensions
    {
        public static string SrpSession(string login) => $"crossdyne:srp:{login}";
    }
}