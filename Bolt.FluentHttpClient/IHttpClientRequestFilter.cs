using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Bolt.FluentHttpClient
{
    public interface IHttpClientRequestFilter
    {
        ValueTask Filter(HttpRequestMessage msg, CancellationToken cancellationToken);
        ValueTask Filter(HttpResponseMessage msg, CancellationToken cancellationToken);
    }

    public abstract class HttpClientRequestFilter : IHttpClientRequestFilter
    {
        public virtual ValueTask Filter(HttpRequestMessage msg, CancellationToken cancellationToken) { return new ValueTask(); }
        public virtual ValueTask Filter(HttpResponseMessage msg, CancellationToken cancellationToken) { return new ValueTask(); }
    }
}
