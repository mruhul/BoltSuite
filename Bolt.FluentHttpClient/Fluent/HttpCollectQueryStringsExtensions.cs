using System.Collections.Generic;
using System.Linq;

namespace Bolt.FluentHttpClient.Fluent
{
    public static class HttpCollectQueryStringsExtensions
    {
        /// <summary>
        /// Add query strings to request url from a object. 
        /// The method read all first level properties and their values and append as 
        /// querystring with propertyname as querystring name and propertyvalue as query 
        /// value if the value is not null.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="data"></param>
        /// <param name="isValueEncoded"></param>
        /// <returns></returns>
        public static IHttpHaveQueryStrings QueryStrings<T>(this IHttpCollectQueryStrings source, T data, bool isValueEncoded = false)
        {
            return source.QueryStrings(NameValueUnitCollectionMapper.FromDto(data), isValueEncoded);
        }

        /// <summary>
        /// Add querystrings from a <see cref="Dictionary{string, string}"/>
        /// </summary>
        /// <param name="source"></param>
        /// <param name="data"></param>
        /// <param name="isValueEncoded"></param>
        /// <returns></returns>
        public static IHttpHaveQueryStrings QueryStrings(this IHttpCollectQueryStrings source, Dictionary<string,string> data, bool isValueEncoded = false)
        {
            return source.QueryStrings(NameValueUnitCollectionMapper.FromDictionary(data), isValueEncoded);
        }

        /// <summary>
        /// Add querystrings from <see cref="IDictionary{string, string}"/>
        /// </summary>
        /// <param name="source"></param>
        /// <param name="data"></param>
        /// <param name="isValueEncoded"></param>
        /// <returns></returns>
        public static IHttpHaveQueryStrings QueryStrings(this IHttpCollectQueryStrings source, IDictionary<string, string> data, bool isValueEncoded = false)
        {
            return source.QueryStrings(NameValueUnitCollectionMapper.FromDictionary(data), isValueEncoded);
        }
    }
}
