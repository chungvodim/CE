using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGP.Utils.CacheManager
{
    public interface ICacheManager
    {
        void Set(string key, object value);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="duration">in minutes</param>
        void Set(string key, object value, int duration);

        void Set(string key, object value, TimeSpan duration);

        void Set(string key, object value, DateTime expiration);

        object Get(string key);

        T Get<T>(string key);

        T GetOrUpdate<T>(string key, Func<T> valueFactory);

        Task<T> GetOrUpdateAsync<T>(string key, Func<Task<T>> valueFactory);

        T GetOrUpdate<T>(string key, Func<T> valueFactory, int duration);

        Task<T> GetOrUpdateAsync<T>(string key, Func<Task<T>> valueFactory, int duration);

        T GetOrUpdate<T>(string key, Func<T> valueFactory, TimeSpan duration);

        Task<T> GetOrUpdateAsync<T>(string key, Func<Task<T>> valueFactory, TimeSpan duration);

        T GetOrUpdate<T>(string key, Func<T> valueFactory, DateTime expiration);

        Task<T> GetOrUpdateAsync<T>(string key, Func<Task<T>> valueFactory, DateTime expiration);

        IDictionary<string, object> Get(string[] keys);

        IDictionary<string, object> Get();

        IEnumerable<string> GetKeys();

        void Remove(string key);
    }
}
