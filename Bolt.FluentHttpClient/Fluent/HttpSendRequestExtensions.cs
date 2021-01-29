using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Bolt.FluentHttpClient.Fluent
{
    public static class HttpSendRequestExtensions
    {
        /// <summary>
        /// Perform an http request for GET method
        /// </summary>
        /// <param name="source"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static Task<HttpResponseMessage> GetRequestAsync(this IHttpSendRequest source, CancellationToken cancellationToken = default)
        {
            return source.SendRequestAsync(HttpMethod.Get, cancellationToken);
        }

        /// <summary>
        /// Perform an http request for DELETE method
        /// </summary>
        /// <param name="source"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static Task<HttpResponseMessage> DeleteRequestAsync(this IHttpSendRequest source, CancellationToken cancellationToken = default)
        {
            return source.SendRequestAsync(HttpMethod.Delete, cancellationToken);
        }

        /// <summary>
        /// Perform an http request for POST method
        /// </summary>
        /// <param name="source"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static Task<HttpResponseMessage> PostRequestAsync(this IHttpSendRequest source, CancellationToken cancellationToken = default)
        {
            return source.SendRequestAsync(HttpMethod.Post, cancellationToken);
        }

        /// <summary>
        /// Perform an http request for POST method with content
        /// </summary>
        /// <param name="source"></param>
        /// <param name="content"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static Task<HttpResponseMessage> PostRequestAsync(this IHttpSendRequest source, HttpContent content, CancellationToken cancellationToken = default)
        {
            return source.SendRequestAsync(HttpMethod.Post, content, cancellationToken);
        }

        /// <summary>
        /// Perform an http request for PUT method
        /// </summary>
        /// <param name="source"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static Task<HttpResponseMessage> PutRequestAsync(this IHttpSendRequest source, CancellationToken cancellationToken = default)
        {
            return source.SendRequestAsync(HttpMethod.Put, cancellationToken);
        }

        /// <summary>
        /// Perform an http request for PUT method with content
        /// </summary>
        /// <param name="source"></param>
        /// <param name="content"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static Task<HttpResponseMessage> PutRequestAsync(this IHttpSendRequest source, HttpContent content, CancellationToken cancellationToken = default)
        {
            return source.SendRequestAsync(HttpMethod.Put, content, cancellationToken);
        }
    }
}
