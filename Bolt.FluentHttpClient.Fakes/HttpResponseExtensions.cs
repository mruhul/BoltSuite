using Bolt.FluentHttpClient.Fluent;
using System.Linq;

namespace Bolt.FluentHttpClient.Fakes
{
    public static class HttpResponseExtensions
    {
        public static bool IsFakeResponse(this IHttpResponse rsp) 
            => rsp.Headers.Any(x => x.Key == "x-fake-response" 
                                    && x.Value == "true");
    }
}
