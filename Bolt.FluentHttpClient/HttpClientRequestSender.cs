using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Bolt.FluentHttpClient
{
    internal sealed class HttpClientRequestSender
    {
        private readonly IHttpClientWrapper clientWrapper;
        private readonly IEnumerable<IHttpClientRequestFilter> filters;

        public HttpClientRequestSender(IHttpClientWrapper clientWrapper, IEnumerable<IHttpClientRequestFilter> filters)
        {
            this.clientWrapper = clientWrapper;
            this.filters = filters;
        }

        public async Task<HttpResponseMessage> SendAsync(HttpClient client, HttpRequestMessage request, CancellationToken cancellationToken)
        {
            foreach (var filter in filters)
            {
                await filter.Filter(request, cancellationToken);
            }

            var rsp = await clientWrapper.SendAsync(client, request, cancellationToken);

            try
            {
                foreach (var filter in filters)
                {
                    await filter.Filter(rsp, cancellationToken);
                }
            }
            catch
            {
                rsp.Dispose();

                throw;
            }

            return rsp;
        }
    }
}
