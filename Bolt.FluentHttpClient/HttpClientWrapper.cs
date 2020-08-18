using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Bolt.FluentHttpClient
{
    public interface IHttpClientWrapper
    {
        Task<HttpResponseMessage> SendAsync(HttpClient client, HttpRequestMessage request, CancellationToken cancellation);
    }

    public class HttpClientWrapper : IHttpClientWrapper
    {
        public Task<HttpResponseMessage> SendAsync(HttpClient client, HttpRequestMessage request, CancellationToken cancellation)
        {
            return client.SendAsync(request, cancellation);
        }
    }
}
