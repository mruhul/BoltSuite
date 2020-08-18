using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Bolt.FluentHttpClient.Fluent
{
    public interface IHttpSendTypedRequest
    {
        Task<IHttpResponse> SendAsync(HttpMethod method, CancellationToken cancellationToken = default);
        Task<IHttpResponse<TContent>> SendAsync<TContent>(HttpMethod method, CancellationToken cancellationToken = default);
        Task<IHttpResponse> SendAsync<TInput>(HttpMethod method, TInput content, string contentType = null, CancellationToken cancellationToken = default);
        Task<IHttpResponse<TOutput>> SendAsync<TInput, TOutput>(HttpMethod method, TInput content, string contentType = null, CancellationToken cancellationToken = default);
    }
}
