using System;
using System.Threading.Tasks;

namespace Bolt.App.Core
{
    /// <summary>
    /// Provide an interface to store any expensive task result in scope context so that in same scope
    /// when the same data required from another code flow it can reuse the same value 
    /// </summary>
    public interface IContextStore
    {
        /// <summary>
        /// Return T if available in context otherwise return default T
        /// </summary>
        /// <param name="key"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T Get<T>(string key);
        
        /// <summary>
        /// Return whether an item with supplied key exists in context or not
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        bool Exists(string key);
        /// <summary>
        /// Store an object against a key in context
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void Set(string key, object value);
        /// <summary>
        /// Remove item from context based on key if exists
        /// </summary>
        /// <param name="key"></param>
        void Remove(string key);

        /// <summary>
        /// Return existing item of T if available in context otherwise execute the fetch func to set the item in context
        /// and return that item. If lock true then whole fetch run inside lock so that no other process start same fetch
        /// again.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="fetch"></param>
        /// <param name="useLock"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T GetOrFetch<T>(string key, Func<T> fetch, bool useLock);
        
        /// <summary>
        /// Return existing item of T if available in context otherwise execute the fetch func to set the item in context
        /// and return that item. If lock true then whole fetch run inside lock so that no other process start same fetch
        /// again.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="fetch"></param>
        /// <param name="useLock"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        ValueTask<T> GetOrFetchAsync<T>(string key, Func<Task<T>> fetch, bool useLock);
    }
}