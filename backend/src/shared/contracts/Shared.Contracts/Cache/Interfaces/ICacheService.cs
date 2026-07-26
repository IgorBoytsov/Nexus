namespace Shared.Contracts.Cache.Interfaces
{
    public interface ICacheService
    {
        Task<bool> SetStringAsync(string key, string value, TimeSpan? expiry = null);
        Task<string?> GetStringAsync(string key);
        Task<bool> RemoveAsync(string key);

        Task<bool> SetJsonAsync<T>(string key, T value, TimeSpan? expiry = null);
        Task<T?> GetJsonAsync<T>(string key);

        Task<bool> SetAddAsync(string key, string value, TimeSpan? expiry = null);
        Task<string[]> SetMembersAsync(string key);
        Task<bool> SetRemoveAsync(string key, string value);
        Task<long> RemoveAsync(string[] keys);
    }
}