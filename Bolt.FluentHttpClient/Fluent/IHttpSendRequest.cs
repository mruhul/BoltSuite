using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Bolt.FluentHttpClient.Fluent
{
    public interface IHttpSendRequest
    {
        Task<HttpResponseMessage> SendRequestAsync(HttpMethod method, CancellationToken cancellationToken = default);
        Task<HttpResponseMessage> SendRequestAsync(HttpMethod method, HttpContent content, CancellationToken cancellationToken = default);
    }
}
