using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Bolt.FluentHttpClient
{
    public interface ITypedHttpClient
    {
        Task<IHttpResponse> SendAsync(HttpClient client, 
            HttpRequestDto request, 
            Func<HttpResponseMessage, CancellationToken, Task> onError = null,
            CancellationToken cancellationToken = default);

        Task<IHttpResponse> SendAsync<TRequest>(HttpClient client, 
            HttpRequestDto<TRequest> request, 
            Func<HttpResponseMessage, CancellationToken, Task> onError = null, 
            CancellationToken cancellationToken = default);

        Task<IHttpResponse<TResponse>> SendAsync<TResponse>(HttpClient client, 
            HttpRequestDto request, 
            Func<HttpResponseMessage, CancellationToken, Task> onError = null, 
            CancellationToken cancellationToken = default);

        Task<IHttpResponse<TResponse>> SendAsync<TRequest, TResponse>(HttpClient client,
            HttpRequestDto<TRequest> request,
            Func<HttpResponseMessage, CancellationToken, Task> onError = null,
            CancellationToken cancellationToken = default);
    }

    internal sealed class TypedHttpClient : ITypedHttpClient
    {
        private readonly HttpClientRequestSender httpRequestSender;
        private readonly IEnumerable<IHttpContentSerializer> serializers;
        private static readonly Type NoneType = typeof(None);

        public TypedHttpClient(HttpClientRequestSender httpRequestSender,
            IEnumerable<IHttpContentSerializer> serializers)
        {
            this.httpRequestSender = httpRequestSender;
            this.serializers = serializers;
        }

        public async Task<IHttpResponse> SendAsync(HttpClient client, 
            HttpRequestDto request, 
            Func<HttpResponseMessage, CancellationToken, Task> onError, 
            CancellationToken cancellationToken)
        {
            return await SendAsync<None, None>(client, ToNoneTypedRequest(request), onError, cancellationToken);
        }

        public async Task<IHttpResponse> SendAsync<TRequest>(HttpClient client, 
            HttpRequestDto<TRequest> request, 
            Func<HttpResponseMessage, CancellationToken, Task> onError, 
            CancellationToken cancellationToken)
        {
            return await SendAsync<TRequest, None>(client, request, onError, cancellationToken);
        }

        public Task<IHttpResponse<TResponse>> SendAsync<TResponse>(HttpClient client, 
            HttpRequestDto request, 
            Func<HttpResponseMessage, CancellationToken, Task> onError, 
            CancellationToken cancellationToken)
        {
            return SendAsync<None, TResponse>(client, ToNoneTypedRequest(request) , onError, cancellationToken);
        }

        public async Task<IHttpResponse<TResponse>> SendAsync<TRequest, TResponse>(HttpClient client, 
            HttpRequestDto<TRequest> request, 
            Func<HttpResponseMessage, CancellationToken, Task> onError, 
            CancellationToken cancellationToken)
        {
            using var streamContent = await BuildContent(request.Content, request.ContentType, cancellationToken);

            using var rsp = await this.SendRequestAsync(client, request, streamContent, cancellationToken);

            return await BuildResponseDto<TResponse>(rsp, onError, cancellationToken);
        }


        private HttpRequestDto<None> ToNoneTypedRequest(HttpRequestDto request)
        {
            return new HttpRequestDto<None>
            {
                Content = null,
                ContentType = null,
                Headers = request.Headers,
                Method = request.Method,
                RetryCount = request.RetryCount,
                Timeout = request.Timeout,
                Uri = request.Uri
            };
        }


        private Task<HttpResponseMessage> SendRequestAsync(HttpClient client, 
            HttpRequestDto request, 
            HttpContent content, 
            CancellationToken cancellationToken)
        {
            return httpRequestSender.SendAsync(client, BuildMessage(request, content), cancellationToken);
        }

        private async ValueTask<StreamContent> BuildContent<TInput>(TInput input, string contentType, CancellationToken cancellationToken)
        {
            if (input == null || typeof(TInput) == NoneType) return null;

            contentType = contentType ?? ContentTypeJson;

            var serializer = AppliedSeriaizer(contentType);

            var ms = new MemoryStream();

            await serializer.SerializeAsync(ms, input, cancellationToken);

            var streamContent = new StreamContent(ms);

            streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(contentType);

            return streamContent;
        }

        private async ValueTask<IHttpResponse<T>> BuildResponseDto<T>(HttpResponseMessage rspMsg, 
            Func<HttpResponseMessage, CancellationToken, Task> onFailure,
            CancellationToken cancellationToken)
        {
            var rsp = new HttpResponseDto<T>();

            PopulateResponseDto(rsp, rspMsg);

            if (rspMsg.IsSuccessStatusCode)
            {
                rsp.Content = await ReadContentAsync<T>(rspMsg, cancellationToken);
            }
            else
            {
                if (onFailure != null)
                {
                    await onFailure(rspMsg, cancellationToken);
                }
            }

            return rsp;
        }

        private HttpRequestMessage BuildMessage(HttpRequestDto request, HttpContent content)
        {
            var msg = new HttpRequestMessage(request.Method, request.Uri);

            if (request.Headers != null)
            {
                foreach (var header in request.Headers)
                {
                    msg.Headers.Add(header.Key, header.Value);
                }
            }

            if (request.Timeout.HasValue && request.Timeout != TimeSpan.Zero) msg.Properties.Add(Constants.PropertyTimeoutInMs, request.Timeout);
            if (request.RetryCount.HasValue && request.RetryCount > 0) msg.Properties.Add(Constants.PropertyRetryCount, request.RetryCount);

            if (content != null) msg.Content = content;

            return msg;
        }

        private async Task<TContent> ReadContentAsync<TContent>(HttpResponseMessage rspMsg, CancellationToken cancellationToken)
        {
            if (rspMsg.Content == null || typeof(TContent) == NoneType) return default;

            using var cnt = rspMsg.Content;

            using var sr = await cnt.ReadAsStreamAsync();

            var serializer = AppliedSeriaizer(cnt.Headers?.ContentType?.MediaType);

            return await serializer.DeserializeAsync<TContent>(sr, cancellationToken);
        }

        private const string ContentTypeJson = "application/json";
        private IHttpContentSerializer AppliedSeriaizer(string contentType)
            => serializers.FirstOrDefault(x => x.IsApplicable(contentType ?? ContentTypeJson))
            ?? serializers.FirstOrDefault();


        private void PopulateResponseDto(HttpResponseDto rsp, HttpResponseMessage sourceRspMsg)
        {
            rsp.StatusCode = sourceRspMsg.StatusCode;
            rsp.IsSuccessStatusCode = sourceRspMsg.IsSuccessStatusCode;

            var headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            if (sourceRspMsg.Headers != null)
            {
                foreach (var header in sourceRspMsg.Headers)
                {
                    headers[header.Key] = header.Value == null ? string.Empty : string.Join(",", header.Value);
                }
            }

            rsp.Headers = headers;
        }

        class None { }
    }
}
