using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Bolt.FluentHttpClient
{
    public interface IHttpClientRequestFilter
    {
        ValueTask Filter(HttpRequestMessage msg, CancellationToken cancellationToken);
    }
}
