using System;
using System.Threading.Tasks;

namespace Bolt.App.Core
{
    public interface IContextStore
    {
        T Get<T>(string key);
        bool Exists(string key);
        void Set(string key, object value);
        void Remove(string key);

        T GetOrFetch<T>(string key, Func<T> fetch, bool useLock);
        ValueTask<T> GetOrFetchAsync<T>(string key, Func<Task<T>> fetch, bool useLock);
    }
}