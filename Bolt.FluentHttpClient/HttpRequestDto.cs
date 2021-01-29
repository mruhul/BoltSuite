using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Bolt.FluentHttpClient
{
    public class HttpRequestDto
    {
        public Uri Uri { get; set; }
        public int? RetryCount { get; set; }
        public TimeSpan? Timeout { get; set; }
        public HttpMethod Method { get; set; }
        public Dictionary<string,string> Headers { get; set; }
    }

    public class HttpRequestDto<T> : HttpRequestDto
    {
        public string ContentType { get; set; }
        public T Content { get; set; }
    }
}
