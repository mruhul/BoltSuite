using System.Collections.Generic;

namespace Bolt.FluentHttpClient.Fluent
{

    public static class HttpCollectHeadersExtensions
    {
        /// <summary>
        /// Add headers to request from a <see cref="Dictionary{string, string}"/>
        /// </summary>
        /// <param name="source"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static IHttpHaveHeaders Headers(this IHttpCollectHeaders source, Dictionary<string, string> data)
        {
            return source.Headers(NameValueUnitCollectionMapper.FromDictionary(data));
        }

        /// <summary>
        /// Add headers to request from a <see cref="IDictionary{string, string}"/>
        /// </summary>
        /// <param name="source"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static IHttpHaveHeaders Headers(this IHttpCollectHeaders source, IDictionary<string, string> data)
        {
            return source.Headers(NameValueUnitCollectionMapper.FromDictionary(data));
        }

        /// <summary>
        /// Add auth bearer token
        /// </summary>
        /// <param name="source"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static IHttpHaveHeaders AuthBearerToken(this IHttpHaveHeaders source, string token)
        {
            return source.AuthHeader($"Bearer {token}");
        }

        /// <summary>
        /// Add auth header
        /// </summary>
        /// <param name="source"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static IHttpHaveHeaders AuthHeader(this IHttpHaveHeaders source, string value)
        {
            return source.Header("Authorization", value);
        }
    }
}
