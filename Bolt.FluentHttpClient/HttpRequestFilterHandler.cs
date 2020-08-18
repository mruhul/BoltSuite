using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Bolt.FluentHttpClient
{
    internal sealed class HttpRequestFilterHandler : DelegatingHandler
    {
        private readonly IEnumerable<IHttpClientRequestFilter> filters;

        public HttpRequestFilterHandler(IEnumerable<IHttpClientRequestFilter> filters)
        {
            this.filters = filters;
        }

        protected async override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, 
            CancellationToken cancellationToken)
        {
            if(filters != null)
            {
                foreach(var filter in filters)
                {
                    await filter.Filter(request, cancellationToken);
                }
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
