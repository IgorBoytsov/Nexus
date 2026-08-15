using System.Text.Json;
using Shared.Contracts.Cache.Abstractions;

namespace Shared.Test.Cache
{
    public sealed class InMemoryCacheService : ICacheService
    {
        private readonly Dictionary<string, (string Value, DateTime? ExpiresAt)> _store = new();
        private readonly Dictionary<string, HashSet<string>> _sets = new();

        private static readonly JsonSerializerOptions _jsonOpts = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public Task<bool> SetStringAsync(string key, string value, TimeSpan? expiry = null)
        {
            var expires = expiry.HasValue ? DateTime.UtcNow.Add(expiry.Value) : (DateTime?)null;
            _store[key] = (value, expires);
            return Task.FromResult(true);
        }

        public Task<string?> GetStringAsync(string key)
        {
            if (_store.TryGetValue(key, out var entry))
            {
                if (entry.ExpiresAt == null || entry.ExpiresAt > DateTime.UtcNow)
                    return Task.FromResult<string?>(entry.Value);
                _store.Remove(key);
            }
            return Task.FromResult<string?>(null);
        }

        public Task<bool> RemoveAsync(string key)
        {
            var removed = _store.Remove(key);
            _sets.Remove(key);
            return Task.FromResult(removed);
        }

        public Task<bool> SetJsonAsync<T>(string key, T value, TimeSpan? expiry = null)
        {
            var json = JsonSerializer.Serialize(value, _jsonOpts);
            return SetStringAsync(key, json, expiry);
        }

        public Task<T?> GetJsonAsync<T>(string key)
        {
            var json = GetStringAsync(key).GetAwaiter().GetResult();
            if (json == null) return Task.FromResult<T?>(default);
            return Task.FromResult(JsonSerializer.Deserialize<T>(json, _jsonOpts));
        }

        public Task<bool> SetAddAsync(string key, string value, TimeSpan? expiry = null)
        {
            if (!_sets.TryGetValue(key, out var set))
            {
                set = new HashSet<string>();
                _sets[key] = set;
            }
            return Task.FromResult(set.Add(value));
        }

        public Task<string[]> SetMembersAsync(string key)
        {
            if (_sets.TryGetValue(key, out var set))
                return Task.FromResult(set.ToArray());
            return Task.FromResult(Array.Empty<string>());
        }

        public Task<bool> SetRemoveAsync(string key, string value)
        {
            if (_sets.TryGetValue(key, out var set))
                return Task.FromResult(set.Remove(value));
            return Task.FromResult(false);
        }

        public Task<long> RemoveAsync(string[] keys)
        {
            long count = 0;
            foreach (var key in keys)
            {
                if (_store.Remove(key) || _sets.Remove(key))
                    count++;
            }
            return Task.FromResult(count);
        }
    }
}