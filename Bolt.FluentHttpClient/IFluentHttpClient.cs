using Bolt.FluentHttpClient.Fluent;
using System.Net.Http;

namespace Bolt.FluentHttpClient
{
    public interface IFluentHttpClient
    {
        /// <summary>
        /// You can pass existing httpclient to use. This is optional.
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        IHttpHaveClient UseClient(HttpClient client);

        /// <summary>
        /// Set the url you like to send request
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        IHttpHaveUrl ForUrl(string url);
    }
}
