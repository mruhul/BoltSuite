using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Bolt.FluentHttpClient.Fluent
{
    public static class HttpSendRequestExtensions
    {
        public static Task<HttpResponseMessage> GetRequestAsync(this IHttpSendRequest source, CancellationToken cancellationToken = default)
        {
            return source.SendRequestAsync(HttpMethod.Get, cancellationToken);
        }

        public static Task<HttpResponseMessage> DeleteRequestAsync(this IHttpSendRequest source, CancellationToken cancellationToken = default)
        {
            return source.SendRequestAsync(HttpMethod.Delete, cancellationToken);
        }

        public static Task<HttpResponseMessage> PostRequestAsync(this IHttpSendRequest source, CancellationToken cancellationToken = default)
        {
            return source.SendRequestAsync(HttpMethod.Post, cancellationToken);
        }

        public static Task<HttpResponseMessage> PostRequestAsync(this IHttpSendRequest source, HttpContent content, CancellationToken cancellationToken = default)
        {
            return source.SendRequestAsync(HttpMethod.Post, content, cancellationToken);
        }

        public static Task<HttpResponseMessage> PutRequestAsync(this IHttpSendRequest source, CancellationToken cancellationToken = default)
        {
            return source.SendRequestAsync(HttpMethod.Put, cancellationToken);
        }

        public static Task<HttpResponseMessage> PutRequestAsync(this IHttpSendRequest source, HttpContent content, CancellationToken cancellationToken = default)
        {
            return source.SendRequestAsync(HttpMethod.Put, content, cancellationToken);
        }
    }
}
