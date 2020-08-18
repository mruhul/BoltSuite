using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Bolt.FluentHttpClient
{
    internal sealed class HttpTimeoutHandler : DelegatingHandler
    {
        private readonly ILogger<HttpTimeoutHandler> logger;

        public HttpTimeoutHandler(ILogger<HttpTimeoutHandler> logger)
        {
            this.logger = logger;
        }

        protected async override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            TimeSpan timeout = request.Properties.TryGetValue(Constants.PropertyTimeoutInMs, out var to)
                                ? (TimeSpan)to : TimeSpan.Zero;

            if (timeout == TimeSpan.Zero)
            {
                return await base.SendAsync(request, cancellationToken);
            }

            using var tokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            tokenSource.CancelAfter(timeout);

            try
            {
                logger.LogTrace($"Start sending request {request.Method}:{request.RequestUri}");

                var rsp = await base.SendAsync(request, tokenSource.Token);

                logger.LogTrace($"Response received for request {request.Method}:{request.RequestUri}");

                return rsp;
            }
            catch (OperationCanceledException)
            {
                var timeoutInMs = timeout.TotalMilliseconds;

                logger.LogError($"Request timeout after {timeoutInMs}ms for url {request.Method}:{request.RequestUri}");

                return BuildTimeoutMessage(timeoutInMs);
            }
        }

        private HttpResponseMessage BuildTimeoutMessage(double timeoutInMs)
        {
            var rsp = new HttpResponseMessage
            {
                StatusCode = System.Net.HttpStatusCode.RequestTimeout,
                ReasonPhrase = "RequestTimeout"
            };

            rsp.Headers.Add("x-httprequest-error", $"Timeout after {timeoutInMs}ms");

            return rsp;
        }
    }
}
