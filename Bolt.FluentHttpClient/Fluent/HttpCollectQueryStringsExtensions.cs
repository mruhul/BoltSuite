using System.Collections.Generic;
using System.Linq;

namespace Bolt.FluentHttpClient.Fluent
{
    public static class HttpCollectQueryStringsExtensions
    {
        public static IHttpHaveQueryStrings QueryStrings<T>(this IHttpCollectQueryStrings source, T data, bool isValueEncoded = false)
        {
            return source.QueryStrings(NameValueUnitCollectionMapper.FromDto(data), isValueEncoded);
        }

        public static IHttpHaveQueryStrings QueryStrings<T>(this IHttpCollectQueryStrings source, Dictionary<string,string> data, bool isValueEncoded = false)
        {
            return source.QueryStrings(NameValueUnitCollectionMapper.FromDictionary(data), isValueEncoded);
        }

        public static IHttpHaveQueryStrings QueryStrings<T>(this IHttpCollectQueryStrings source, IDictionary<string, string> data, bool isValueEncoded = false)
        {
            return source.QueryStrings(NameValueUnitCollectionMapper.FromDictionary(data), isValueEncoded);
        }
    }
}
