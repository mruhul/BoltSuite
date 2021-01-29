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
        private readonly ITypedHttpClient typedHttpClient;
        private readonly HttpClient defaultClient;

        public FluentHttp(IEnumerable<IHttpContentSerializer> serializers, 
            IHttpClientWrapper httpClientWrapper,
            ITypedHttpClient typedHttpClient,
            HttpClient defaultClient)
        {
            this.serializers = serializers;
            this.httpClientWrapper = httpClientWrapper;
            this.typedHttpClient = typedHttpClient;
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

                
        public Task<IHttpResponse> SendAsync(HttpMethod method, CancellationToken cancellationToken)
        {
            return typedHttpClient.SendAsync(client ?? defaultClient,
                BuildRequestDto(method),
                onFailure, cancellationToken);
        }

        public Task<IHttpResponse<TOutput>> SendAsync<TOutput>(HttpMethod method, CancellationToken cancellationToken)
        {
            return typedHttpClient.SendAsync<TOutput>(client ?? defaultClient,
                BuildRequestDto(method),
                onFailure, cancellationToken);
        }

        public Task<IHttpResponse> SendAsync<TInput>(HttpMethod method, 
            TInput content, 
            string contentType, 
            CancellationToken cancellationToken)
        {
            return typedHttpClient.SendAsync(client ?? defaultClient,
                BuildRequestDto(method, content, contentType),
                onFailure, cancellationToken);
        }

        public Task<IHttpResponse<TOutput>> SendAsync<TInput, TOutput>(HttpMethod method, 
            TInput content, 
            string contentType, 
            CancellationToken cancellationToken)
        {
            return typedHttpClient.SendAsync<TInput, TOutput>(client ?? defaultClient,
                BuildRequestDto(method, content, contentType),
                onFailure, cancellationToken);
        }

        private HttpRequestDto BuildRequestDto(HttpMethod method)
        {
            return new HttpRequestDto
            {
                Headers = BuildHeaders(headers),
                Method = method,
                RetryCount = retry,
                Timeout = timeout,
                Uri = new Uri(url)
            };
        }

        private HttpRequestDto<TContent> BuildRequestDto<TContent>(HttpMethod method, TContent content, string contentType)
        {
            return new HttpRequestDto<TContent>
            {
                Content = content,
                ContentType = contentType,
                Headers = BuildHeaders(headers),
                Method = method,
                RetryCount = retry,
                Timeout = timeout,
                Uri = new Uri(url)
            };
        }

        private Dictionary<string,string> BuildHeaders(List<NameValueUnit> source)
        {
            if (source == null || source.Count == 0) return null;

            var result = new Dictionary<string, string>();

            foreach(var item in source)
            {
                result[item.Name] = item.Value;
            }

            return result;
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
