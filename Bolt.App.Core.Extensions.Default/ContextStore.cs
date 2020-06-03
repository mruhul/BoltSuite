using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Bolt.App.Core.Extensions.Default
{
    public class ContextStore : IContextStore
    {
        private readonly ConcurrentDictionary<string, object> _store = new ConcurrentDictionary<string, object>();
        private readonly ConcurrentDictionary<string, SemaphoreSlim> _asyncLocks = new ConcurrentDictionary<string, SemaphoreSlim>();
        private readonly ConcurrentDictionary<string, object> _locks = new ConcurrentDictionary<string, object>();
        
        public T Get<T>(string key)
        {
            return _store.TryGetValue(key, out object result) ? (T)result : default;
        }

        public bool Exists(string key)
        {
            return _store.ContainsKey(key);
        }

        public void Set(string key, object value)
        {
            _store.AddOrUpdate(key, value, (k, n) => value);
        }

        public void Remove(string key)
        {
            _store.TryRemove(key, out object _);
        }

        public T GetOrFetch<T>(string key, Func<T> fetch, bool useLock)
        {
            if(TryGet<T>(key, out var result))
            {
                return result;
            }

            if (!useLock)
            {
                return SetAndReturn(key, fetch);
            }

            var lockObj = _locks.GetOrAdd(key, k => new object());

            lock(lockObj)
            {
                return TryGet<T>(key, out var existingResult) 
                    ? existingResult 
                    : SetAndReturn(key, fetch);
            }
        }

        public async ValueTask<T> GetOrFetchAsync<T>(string key, Func<Task<T>> fetch, bool useLock)
        {
            if (TryGet<T>(key, out var result))
            {
                return result;
            }

            if (!useLock)
            {
                return await SetAndReturn(key, fetch);
            }

            var lockObj = _asyncLocks.GetOrAdd(key, k => new SemaphoreSlim(1,1));

            await lockObj.WaitAsync();

            try
            {
                if (TryGet<T>(key, out var existingResult))
                {
                    return existingResult;
                }

                return await SetAndReturn(key, fetch);
            }
            finally
            {
                lockObj.Release();
            }
        }

        private bool TryGet<T>(string key, out T value)
        {
            if(_store.TryGetValue(key, out var result))
            {
                value = result == null ? default : (T)result;
                return true;
            }

            value = default;

            return false;
        }

        private T SetAndReturn<T>(string key, Func<T> fetch)
        {
            var result = fetch.Invoke();

            _store.AddOrUpdate(key, result, (k, n) => result);

            return result;
        }

        private async Task<T> SetAndReturn<T>(string key, Func<Task<T>> fetch)
        {
            var result = await fetch.Invoke();

            _store.AddOrUpdate(key, result, (k, n) => result);

            return result;
        }
    }
}