using Bolt.FluentHttpClient.Fluent;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Bolt.FluentHttpClient.Fluent
{
    internal sealed class FluentHttp : 
        IFluentHttpClient,
        IHttpCollectClient,
        IHttpHaveClient,
        IHttpHaveUrl,
        IHttpHaveHeaders,
        IHttpHaveQueryStrings,
        IHttpHaveRetry,
        IHttpHaveTimeout,
        IHttpHaveOnFailure
    {
        private HttpClient client;
        private string url;
        private List<NameValueUnit> headers;
        private List<NameValueUnit> queryParams;
        private int retry;
        private TimeSpan timeout = TimeSpan.Zero;
        private Func<HttpResponseMessage, CancellationToken, Task> onFailure;
        private readonly IEnumerable<IHttpContentSerializer> serializers;
        private readonly IHttpClientWrapper httpClientWrapper;
        private readonly HttpClient defaultClient;

        public FluentHttp(IEnumerable<IHttpContentSerializer> serializers, IHttpClientWrapper httpClientWrapper, HttpClient defaultClient)
        {
            this.serializers = serializers;
            this.httpClientWrapper = httpClientWrapper;
            this.defaultClient = defaultClient;
        }

        public IHttpHaveClient UseClient(HttpClient client)
        {
            this.client = client;
            return this;
        }

        public IHttpHaveUrl ForUrl(string url)
        {
            this.url = url;
            return this;
        }

        private void AddHeader(NameValueUnit header)
        {
            if (string.IsNullOrWhiteSpace(header.Name)) throw new ArgumentException($"Header {nameof(header.Name)} cannot be null or empty.");

            if (header.Value == null) return;

            headers.Add(header);
        }

        private void AddQueryString(NameValueUnit queryString, bool isValueEncoded)
        {
            if (string.IsNullOrWhiteSpace(queryString.Name)) throw new ArgumentException($"Querystring {nameof(queryString.Name)} cannot be null or empty.");

            if (queryString.Value == null) return;

            if (!isValueEncoded) queryString.Value = Uri.EscapeDataString(queryString.Value);

            queryParams.Add(queryString);
        }

        public IHttpHaveHeaders Header(string name, string value)
        {
            if (headers == null) headers = new List<NameValueUnit>();

            AddHeader(new NameValueUnit { Name = name, Value = value });
            
            return this;
        }

        public IHttpHaveHeaders Headers(NameValueUnit[] headers)
        {
            if (headers == null) throw new ArgumentException($"{nameof(headers)} cannot be null");

            if (this.headers == null)
            {
                this.headers = new List<NameValueUnit>();
            }

            foreach (var header in headers)
            {
                AddHeader(header);
            }

            return this;
        }

        public IHttpHaveQueryStrings QueryString(
            string name, 
            string value, 
            bool isValueEncoded = false)
        {
            if (queryParams == null) queryParams = new List<NameValueUnit>();

            AddQueryString(new NameValueUnit { Name = name, Value = value }, isValueEncoded);

            return this;
        }

        public IHttpHaveQueryStrings QueryStrings(
            NameValueUnit[] queryParams, 
            bool isValueEncoded = false)
        {
            if (queryParams == null) return this;

            if (this.queryParams == null)
            {
                this.queryParams = new List<NameValueUnit>();
            }

            foreach (var unit in queryParams)
            {
                AddQueryString(unit, isValueEncoded);
            }

            return this;
        }

        public IHttpHaveRetry Retry(int retry)
        {
            this.retry = retry;

            return this;
        }

        private HttpRequestMessage BuildMessage(HttpMethod method, HttpContent content)
        {
            var msg = new HttpRequestMessage(method, UrlBuilder.Build(url, queryParams));

            if(headers != null)
            {
                foreach(var header in headers)
                {
                    msg.Headers.Add(header.Name, header.Value);
                }
            }

            if (timeout != TimeSpan.Zero) msg.Properties.Add(Constants.PropertyTimeoutInMs, timeout);
            if (retry > 0) msg.Properties.Add(Constants.PropertyRetryCount, retry);

            if (content != null) msg.Content = content;

            return msg;
        }

        public IHttpHaveTimeout Timeout(TimeSpan timeout)
        {
            this.timeout = timeout;

            return this;
        }


        public IHttpHaveOnFailure OnFailure(Func<HttpStatusCode, Stream, CancellationToken, Task> onFailure)
        {
            this.onFailure = async (rspMsg, ct) =>
            {
                using var sr = rspMsg.Content == null ? null : await rspMsg.Content.ReadAsStreamAsync();

                await onFailure(rspMsg.StatusCode, sr, ct);
            };

            return this;
        }

        public IHttpHaveOnFailure OnFailure(Func<HttpResponseMessage, CancellationToken, Task> onFailure)
        {
            this.onFailure = onFailure;

            return this;
        }

        public IHttpHaveOnFailure OnFailureFromString(Func<HttpStatusCode, string, CancellationToken, Task> onFailure)
        {
            this.onFailure = async (rspMsg, ct) =>
            {
                string content = rspMsg.Content == null ? string.Empty : await rspMsg.Content.ReadAsStringAsync();

                await onFailure(rspMsg.StatusCode, content, ct);
            };

            return this;
        }


        public IHttpHaveOnFailure OnBadRequest<TError>(Action<TError> onBadRequest)
        {
            return this.OnFailure(async (rspMsg, ct) => 
            {
                if (rspMsg.StatusCode != HttpStatusCode.BadRequest) return;

                var err = await ReadContentAsync<TError>(rspMsg, ct);

                onBadRequest(err);
            });
        }

        private async Task<TContent> ReadContentAsync<TContent>(HttpResponseMessage rspMsg, CancellationToken cancellationToken)
        {
            if (rspMsg.Content == null) return default;

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

        private async ValueTask<HttpResponseDto> BuildResponseDto(HttpResponseMessage rspMsg, CancellationToken cancellationToken)
        {
            var rsp = new HttpResponseDto();

            PopulateResponseDto(rsp, rspMsg);

            if (onFailure != null)
            {
                if (!rspMsg.IsSuccessStatusCode)
                {
                    await onFailure(rspMsg, cancellationToken);
                }
            }

            return rsp;
        }

        private async ValueTask<HttpResponseDto<T>> BuildResponseDto<T>(HttpResponseMessage rspMsg, CancellationToken cancellationToken)
        {
            var rsp = new HttpResponseDto<T>();

            PopulateResponseDto(rsp, rspMsg);

            if (rspMsg.IsSuccessStatusCode)
            {
                rsp.Content = await ReadContentAsync<T>(rspMsg, cancellationToken);
            }
            else
            {
                if(onFailure != null)
                {
                    await onFailure(rspMsg, cancellationToken);
                }
            }

            return rsp;
        }

        public async Task<IHttpResponse> SendAsync(HttpMethod method, CancellationToken cancellationToken)
        {
            using var rsp = await this.SendRequestAsync(method, cancellationToken);

            return await BuildResponseDto(rsp, cancellationToken);
        }

        public async Task<IHttpResponse<TOutput>> SendAsync<TOutput>(HttpMethod method, CancellationToken cancellationToken)
        {
            using var rsp = await this.SendRequestAsync(method, cancellationToken);

            return await BuildResponseDto<TOutput>(rsp, cancellationToken);
        }

        public async Task<IHttpResponse> SendAsync<TInput>(HttpMethod method, TInput content, string contentType, CancellationToken cancellationToken)
        {
            using (var streamContent = await BuildContent(content, contentType, cancellationToken))
            {
                using var rsp = await this.SendRequestAsync(method, streamContent, cancellationToken);
                return await BuildResponseDto(rsp, cancellationToken);
            }
        }

        public async Task<IHttpResponse<TOutput>> SendAsync<TInput, TOutput>(HttpMethod method, TInput content, string contentType, CancellationToken cancellationToken)
        {
            using var streamContent = await BuildContent(content, contentType, cancellationToken);

            using var rsp = await this.SendRequestAsync(method, streamContent, cancellationToken);

            return await BuildResponseDto<TOutput>(rsp, cancellationToken);
        }

        private async ValueTask<StreamContent> BuildContent<TInput>(TInput input, string contentType, CancellationToken cancellationToken)
        {
            contentType = contentType ?? ContentTypeJson;

            var serializer = AppliedSeriaizer(contentType);

            var ms = new MemoryStream();

            await serializer.SerializeAsync(ms, input, cancellationToken);

            var streamContent = new StreamContent(ms);

            streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(contentType);

            return streamContent;
        }

        public Task<HttpResponseMessage> SendRequestAsync(HttpMethod method, CancellationToken cancellationToken)
        {
            return this.SendRequestAsync(method, null, cancellationToken);
        }

        public Task<HttpResponseMessage> SendRequestAsync(HttpMethod method, HttpContent content, CancellationToken cancellationToken)
        {
            return httpClientWrapper.SendAsync(client ?? defaultClient, BuildMessage(method, content), cancellationToken);
        }
    }
}
