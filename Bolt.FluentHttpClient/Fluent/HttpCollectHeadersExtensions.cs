using System.Collections.Generic;

namespace Bolt.FluentHttpClient.Fluent
{

    public static class HttpCollectHeadersExtensions
    {
        public static IHttpHaveHeaders Headers(this IHttpCollectHeaders source, Dictionary<string,string> data)
        {
            return source.Headers(NameValueUnitCollectionMapper.FromDictionary(data));
        }

        public static IHttpHaveHeaders Headers(this IHttpCollectHeaders source, IDictionary<string, string> data)
        {
            return source.Headers(NameValueUnitCollectionMapper.FromDictionary(data));
        }

        public static IHttpHaveHeaders AuthBearerToken(this IHttpHaveHeaders source, string token)
        {
            return source.AuthHeader($"Bearer {token}");
        }

        public static IHttpHaveHeaders AuthHeader(this IHttpHaveHeaders source, string value)
        {
            return source.Header("Authorization", value);
        }
    }
}
