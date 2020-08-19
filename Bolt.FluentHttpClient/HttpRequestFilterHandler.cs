using System.Collections.Generic;
using System.Linq;
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
            var hasFilter = filters != null && filters.Any();

            if(!hasFilter) return await base.SendAsync(request, cancellationToken);

            foreach (var filter in filters)
            {
                await filter.Filter(request, cancellationToken);
            }

            HttpResponseMessage rsp = null;

            try
            {
                rsp = await base.SendAsync(request, cancellationToken);

                if (hasFilter)
                {
                    foreach (var filter in filters)
                    {
                        await filter.Filter(rsp, cancellationToken);
                    }
                }

                return rsp;
            }
            catch
            {
                rsp?.Dispose();

                throw;
            }
        }
    }
}
