using System.Collections.Generic;
using System.Net;

namespace Bolt.FluentHttpClient.Fluent
{
    public interface IHttpResponse
    {
        bool IsSuccessStatusCode { get; }
        HttpStatusCode StatusCode { get; }
        Dictionary<string, string> Headers { get; }
    }

    public interface IHttpResponse<TContent> : IHttpResponse
    {
        TContent Content { get; }
    }

    internal class HttpResponseDto : IHttpResponse
    {
        public bool IsSuccessStatusCode { get; internal set; }
        public HttpStatusCode StatusCode { get; internal set; }
        public Dictionary<string, string> Headers { get; internal set; }
    }

    internal sealed class HttpResponseDto<TContent> : HttpResponseDto, IHttpResponse<TContent>
    {
        public TContent Content { get; internal set; }
    }

    public static class HttpResponseExtensions
    {
        public static string Location(this IHttpResponse rsp)
        {
            if (rsp.Headers == null) return string.Empty;
            
            if(rsp.Headers.TryGetValue("Location", out var value))
            {
                return value;
            }

            return string.Empty;
        }
    }
}
