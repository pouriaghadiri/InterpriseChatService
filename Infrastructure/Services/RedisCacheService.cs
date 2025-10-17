using Domain.Services;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Infrastructure.Services
{
    public class RedisCacheService : ICacheService
    {
        private readonly IDistributedCache _distributedCache;
        private readonly JsonSerializerOptions _jsonOptions;

        public RedisCacheService(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false
            };
        }

        public async Task<T?> GetAsync<T>(string key) where T : class
        {
            var cachedValue = await _distributedCache.GetStringAsync(key);
            return cachedValue == null ? null : JsonSerializer.Deserialize<T>(cachedValue, _jsonOptions);
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null) where T : class
        {
            var options = new DistributedCacheEntryOptions();
            if (expiration.HasValue)
                options.SetAbsoluteExpiration(expiration.Value);
            else
                options.SetSlidingExpiration(TimeSpan.FromMinutes(30)); // Default

            var serializedValue = JsonSerializer.Serialize(value, _jsonOptions);
            await _distributedCache.SetStringAsync(key, serializedValue, options);
        }

        public async Task RemoveAsync(string key)
        {
            await _distributedCache.RemoveAsync(key);
        }

        public async Task RemoveByPatternAsync(string pattern)
        {
            // Note: This requires Redis server-side scripting or key enumeration
            // For production, consider using Redis SCAN command
            throw new NotImplementedException("Pattern removal requires Redis SCAN implementation");
        }

        public async Task<bool> ExistsAsync(string key)
        {
            var value = await _distributedCache.GetStringAsync(key);
            return value != null;
        }

        public async Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null) where T : class
        {
            var cachedValue = await GetAsync<T>(key);
            if (cachedValue != null)
                return cachedValue;

            var value = await factory();
            await SetAsync(key, value, expiration);
            return value;
        }

        public async Task<Dictionary<string, T>> GetManyAsync<T>(IEnumerable<string> keys) where T : class
        {
            var result = new Dictionary<string, T>();
            foreach (var key in keys)
            {
                var value = await GetAsync<T>(key);
                if (value != null)
                    result[key] = value;
            }
            return result;
        }

        public async Task SetManyAsync<T>(Dictionary<string, T> values, TimeSpan? expiration = null) where T : class
        {
            foreach (var kvp in values)
            {
                await SetAsync(kvp.Key, kvp.Value, expiration);
            }
        }
    }
}
