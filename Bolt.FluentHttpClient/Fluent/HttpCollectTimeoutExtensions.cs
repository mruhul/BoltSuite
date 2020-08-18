using System;

namespace Bolt.FluentHttpClient.Fluent
{
    public static class HttpCollectTimeoutExtensions
    {
        public static IHttpHaveTimeout TimeoutInMilliseconds(this IHttpCollectTimeout source, int timeoutInMs)
        {
            return source.Timeout(TimeSpan.FromMilliseconds(timeoutInMs));
        }

        public static IHttpHaveTimeout TimeoutInSeconds(this IHttpCollectTimeout source, int seconds)
        {
            return source.Timeout(TimeSpan.FromSeconds(seconds));
        }
    }
}
