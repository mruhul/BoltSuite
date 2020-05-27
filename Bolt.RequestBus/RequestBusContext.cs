using System.Collections.Concurrent;

namespace Bolt.RequestBus
{
    public interface IRequestBusContext
    {
        T GetOrDefault<T>(string name);
        void Set<T>(string name, T value);
        bool Exists(string name);
    }

    public interface IRequestBusContextWriter
    {
        void Write(IRequestBusContext context);
    }

    internal sealed class RequestBusContext : IRequestBusContext
    {
        private readonly ConcurrentDictionary<string,object> _store = new ConcurrentDictionary<string, object>();
        
        public T GetOrDefault<T>(string name)
        {
            return _store.TryGetValue(name, out var result) 
                ? (T) result 
                : default;
        }

        public void Set<T>(string name, T value)
        {
            _store.AddOrUpdate(name, value, (s, o) => value);
        }

        public bool Exists(string name)
        {
            return _store.ContainsKey(name);
        }
    }
}