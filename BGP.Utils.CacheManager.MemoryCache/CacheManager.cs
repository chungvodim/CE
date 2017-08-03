using BGP.Utils.CacheManager;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BGP.Utils.CacheManager.MemoryCache
{
    public class CacheManager : ICacheManager
    {
        static readonly ConcurrentDictionary<string, object> TaskDictionary = new ConcurrentDictionary<string, object>();

        System.Runtime.Caching.MemoryCache _memoryCache;

        public CacheManager(string name, NameValueCollection config = null)
        {
            _memoryCache = new System.Runtime.Caching.MemoryCache(name, config);
        }

        public IDictionary<string, object> Get()
        {
            var result = new Dictionary<string, object>();

            foreach (var cache in _memoryCache)
            {
                result.Add(cache.Key, cache.Value);
            }

            return result;
        }

        public IDictionary<string, object> Get(string[] keys)
        {
            return _memoryCache.GetValues(keys);
        }

        public object Get(string key)
        {
            lock (Lock(key))
            {
                return _memoryCache.Get(key);
            }
        }

        public IEnumerable<string> GetKeys()
        {
            foreach (var cache in _memoryCache)
            {
                yield return cache.Key;
            }
        }

        public T Get<T>(string key)
        {
            lock (Lock(key))
            {
                return (T)_memoryCache.Get(key);
            }
        }

        public T GetOrUpdate<T>(string key, Func<T> valueFactory)
        {
            lock (Lock(key))
            {
                var result = _memoryCache.Get(key);

                if (result == null)
                {
                    result = valueFactory();

                    if (result == null)
                    {
                        throw new Exception("Value factory return null value");
                    }

                    Set(key, result);
                }

                return (T)result;
            }
        }

        public async Task<T> GetOrUpdateAsync<T>(string key, Func<Task<T>> valueFactory)
        {
            var result = _memoryCache.Get(key);

            if (result == null)
            {
                var asyncLazy = (AsyncLazy<T>)TaskDictionary.GetOrAdd(key, (k) =>
                {

                    var newAsyncLazy = new AsyncLazy<T>(k, async (kk) =>
                    {

                        var value = await valueFactory();

                        if (value == null)
                        {
                            throw new Exception("Value factory return null value");
                        }

                        Set(key, value);

                        object oldAsync;
                        TaskDictionary.TryRemove(key, out oldAsync);

                        return (T)value;
                    });

                    return newAsyncLazy;
                });

                return await asyncLazy.Value;
            }

            return (T)result;
        }

        public T GetOrUpdate<T>(string key, Func<T> valueFactory, TimeSpan duration)
        {
            lock (Lock(key))
            {
                var result = _memoryCache.Get(key);

                if (result == null)
                {
                    result = valueFactory();

                    if (result == null)
                    {
                        throw new Exception("Value factory return null value");
                    }

                    Set(key, result, duration);
                }

                return (T)result;
            }
        }

        public T GetOrUpdate<T>(string key, Func<T> valueFactory, DateTime expiration)
        {
            lock (Lock(key))
            {
                var result = _memoryCache.Get(key);

                if (result == null)
                {
                    result = valueFactory();

                    if (result == null)
                    {
                        throw new Exception("Value factory return null value");
                    }

                    Set(key, result, expiration);
                }

                return (T)result;
            }
        }

        public T GetOrUpdate<T>(string key, Func<T> valueFactory, int duration)
        {
            lock (Lock(key))
            {
                var result = _memoryCache.Get(key);

                if (result == null)
                {
                    result = valueFactory();

                    if (result == null)
                    {
                        throw new Exception("Value factory return null value");
                    }

                    Set(key, result, duration);
                }

                return (T)result;
            }
        }

        public async Task<T> GetOrUpdateAsync<T>(string key, Func<Task<T>> valueFactory, DateTime expiration)
        {
            var result = _memoryCache.Get(key);

            if (result == null)
            {
                var asyncLazy = (AsyncLazy<T>)TaskDictionary.GetOrAdd(key, (k) =>
                {

                    var newAsyncLazy = new AsyncLazy<T>(k, async (kk) =>
                    {

                        var value = await valueFactory();

                        if (value == null)
                        {
                            throw new Exception("Value factory return null value");
                        }

                        Set(key, value, expiration);

                        object oldAsync;
                        TaskDictionary.TryRemove(key, out oldAsync);

                        return (T)value;
                    });

                    return newAsyncLazy;
                });

                return await asyncLazy.Value;
            }

            return (T)result;
        }

        public async Task<T> GetOrUpdateAsync<T>(string key, Func<Task<T>> valueFactory, TimeSpan duration)
        {
            var result = _memoryCache.Get(key);

            if (result == null)
            {
                var asyncLazy = (AsyncLazy<T>)TaskDictionary.GetOrAdd(key, (k) =>
                {

                    var newAsyncLazy = new AsyncLazy<T>(k, async (kk) =>
                    {

                        var value = await valueFactory();

                        if (value == null)
                        {
                            throw new Exception("Value factory return null value");
                        }

                        Set(key, value, duration);

                        object oldAsync;
                        TaskDictionary.TryRemove(key, out oldAsync);

                        return (T)value;
                    });

                    return newAsyncLazy;
                });

                return await asyncLazy.Value;
            }

            return (T)result;
        }

        public async Task<T> GetOrUpdateAsync<T>(string key, Func<Task<T>> valueFactory, int duration)
        {
            var result = _memoryCache.Get(key);

            if (result == null)
            {
                var asyncLazy = (AsyncLazy<T>)TaskDictionary.GetOrAdd(key, (k) =>
                {

                    var newAsyncLazy = new AsyncLazy<T>(k, async (kk) =>
                    {

                        var value = await valueFactory();

                        if (value == null)
                        {
                            throw new Exception("Value factory return null value");
                        }

                        Set(key, value, duration);

                        object oldAsync;
                        TaskDictionary.TryRemove(key, out oldAsync);

                        return (T)value;
                    });

                    return newAsyncLazy;
                });

                return await asyncLazy.Value;
            }

            return (T)result;
        }

        public void Remove(string key)
        {
            _memoryCache.Remove(key);
        }

        public void Set(string key, object value)
        {
            _memoryCache.Set(key, value, DateTimeOffset.MaxValue);
        }

        public void Set(string key, object value, DateTime expiration)
        {
            _memoryCache.Set(key, value, new DateTimeOffset(expiration));
        }

        public void Set(string key, object value, TimeSpan duration)
        {
            _memoryCache.Set(key, value, DateTime.Now.Add(duration));
        }

        public void Set(string key, object value, int duration)
        {
            _memoryCache.Set(key, value, DateTime.Now.AddMinutes(duration));
        }

        private string Lock(string key)
        {
            return "_memory_cache_" + key;
        }
    }

}
