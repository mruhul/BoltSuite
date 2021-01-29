using System;

namespace Bolt.FluentHttpClient.Fluent
{
    public static class HttpCollectTimeoutExtensions
    {
        /// <summary>
        /// Set timeout in milliseconds for request
        /// </summary>
        /// <param name="source"></param>
        /// <param name="timeoutInMs"></param>
        /// <returns></returns>
        public static IHttpHaveTimeout TimeoutInMilliseconds(this IHttpCollectTimeout source, int timeoutInMs)
        {
            return source.Timeout(TimeSpan.FromMilliseconds(timeoutInMs));
        }

        /// <summary>
        /// Set timeout in seconds for request
        /// </summary>
        /// <param name="source"></param>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public static IHttpHaveTimeout TimeoutInSeconds(this IHttpCollectTimeout source, int seconds)
        {
            return source.Timeout(TimeSpan.FromSeconds(seconds));
        }
    }
}
