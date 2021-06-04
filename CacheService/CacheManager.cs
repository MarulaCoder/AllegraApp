using Common.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Text.Json;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;

namespace CacheService
{
    public class CacheManager : ICacheManager
    {
        private readonly IDistributedCache _cache;

        public CacheManager(IDistributedCache cache)
        {
            this._cache = cache;
        }

        public async Task<T> GetCached<T>(string Key) where T : class
        {
            if (!String.IsNullOrEmpty(Key))
            {
                byte[] cached_data = await this._cache.GetAsync(Key);

                if (cached_data != null)
                {
                    // convert to object
                    T item = await GetObject<T>(cached_data);
                    return item;
                }
                return (T)null;
            }
            else
            {
                throw new ArgumentNullException("Cache Key can't be null");
            }
        }

        public async Task RefreshCache(string Key)
        {
            if (!String.IsNullOrEmpty(Key))
            {
                await this._cache.RefreshAsync(Key);
            }
        }

        public async Task SetCache<T>(string Key, T entity, TimeSpan timeSpan)
        {
            if (entity != null && !String.IsNullOrEmpty(Key))
            {
                //convert to byte array
                byte[] data = await GetBytes(entity);
                var options = new DistributedCacheEntryOptions()
                                        .SetSlidingExpiration(timeSpan);
                await this._cache.SetAsync(Key, data, options);
            }
            else
            {
                throw new ArgumentNullException("Set Cache can't have null values");
            }
        }

        private async Task<byte[]> GetBytes<T>(T obj)
        {
            if (obj == null)
            {
                return null;
            }
            using (MemoryStream ms = new MemoryStream())
            {
                await JsonSerializer.SerializeAsync(ms, obj);
                return ms.ToArray();
            }
        }

        private async Task<T> GetObject<T>(byte[] data)
        {
            if (data == null)
            {
                return default(T);
            }
            using (MemoryStream ms = new MemoryStream(data))
            {
                object obj = await JsonSerializer.DeserializeAsync<T>(ms);
                return (T)obj;
            }
        }
    }
}
