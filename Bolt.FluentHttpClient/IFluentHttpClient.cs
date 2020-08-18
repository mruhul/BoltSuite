using Bolt.FluentHttpClient.Fluent;
using System.Net.Http;

namespace Bolt.FluentHttpClient
{
    public interface IFluentHttpClient
    {
        IHttpHaveClient UseClient(HttpClient client);
        IHttpHaveUrl ForUrl(string url);
    }
}
