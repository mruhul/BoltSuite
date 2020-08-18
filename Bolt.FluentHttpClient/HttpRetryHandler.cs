using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Bolt.FluentHttpClient
{
    internal sealed class HttpRetryHandler : DelegatingHandler
    {
        private readonly ILogger<HttpRetryHandler> logger;

        public HttpRetryHandler(ILogger<HttpRetryHandler> logger)
        {
            this.logger = logger;
        }

        protected async override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            int maxRequestCount = (request
                    .Properties
                    .TryGetValue(Constants.PropertyRetryCount, out var retry) 
                ? (int)retry : 0) + 1;
            
            HttpResponseMessage rsp = null;

            for(var i = 0; i < maxRequestCount; i++)
            {
                if (rsp != null) rsp.Dispose();

                if (i > 0)
                {
                    logger.LogWarning($"Retrying requst {i} times for url {request.Method}:{request.RequestUri}");
                }

                rsp = await base.SendAsync(request, cancellationToken);

                if (rsp.IsSuccessStatusCode) return rsp;
                
                if (!IsTransientFailure(rsp.StatusCode)) return rsp;
            }

            return rsp;
        }

        private bool IsTransientFailure(HttpStatusCode statusCode)
        {
            return statusCode == HttpStatusCode.RequestTimeout
                    || statusCode == HttpStatusCode.GatewayTimeout
                    || statusCode == HttpStatusCode.InternalServerError
                    || statusCode == HttpStatusCode.ServiceUnavailable;
        }
    }
}
