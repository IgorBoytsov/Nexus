namespace Nexus.Authentication.Service.Application.Common.Abstractions
{
    public interface IRedisCacheService
    {
        Task<bool> SetStringAsync(string key, string value, TimeSpan? expiry = null);
        Task<string?> GetStringAsync(string key);
        Task<bool> RemoveAsync(string key);

        Task<bool> SetJsonAsync<T>(string key, T value, TimeSpan? expiry = null);
        Task<T?> GetJsonAsync<T>(string key);
    }
}