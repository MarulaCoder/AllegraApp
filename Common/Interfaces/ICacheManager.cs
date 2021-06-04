using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Common.Interfaces
{
    public interface ICacheManager
    {
        Task<T> GetCached<T>(string Key) where T : class;
        Task SetCache<T>(string Key, T entity, TimeSpan timeSpan);
        Task RefreshCache(string Key);
    }
}
