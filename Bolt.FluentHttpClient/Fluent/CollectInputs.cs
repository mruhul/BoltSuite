using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Bolt.FluentHttpClient.Fluent
{
    public interface IHttpCollectClient
    {
        /// <summary>
        /// Set this if you like to use a specific httpclient instance. otherwise default httpclient will be used to send request.
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        IHttpHaveClient UseClient(HttpClient client);
    }

    public interface IHttpCollectUrl
    {
        /// <summary>
        /// Set the url you like to send request.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        IHttpHaveUrl ForUrl(string url);
    }

    public interface IHttpCollectQueryStrings
    {
        /// <summary>
        /// Add querystring to request url with supplied name and value.
        /// If a null value supplied the parameter will not be added in querystring and ignored
        /// If value is already encoded please set <paramref name="isValueEncoded"/> to true, otherwise
        /// the value will be encoded for you.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value">We will ignore the query param if the value is null</param>
        /// <param name="isValueEncoded">Indiciates whether value is already encoded or not. If the value is false we will encode the value</param>
        /// <returns></returns>
        IHttpHaveQueryStrings QueryString(string name, string value, bool isValueEncoded = false);

        /// <summary>
        /// Add a collection querystring passed as namevalue units.
        /// </summary>
        /// <param name="queryParams"></param>
        /// <param name="isValueEncoded"></param>
        /// <returns></returns>
        IHttpHaveQueryStrings QueryStrings(NameValueUnit[] queryParams, bool isValueEncoded = false);
    }

    public interface IHttpCollectHeaders
    {
        IHttpHaveHeaders Header(string name, string value);
        IHttpHaveHeaders Headers(NameValueUnit[] headers);
    }

    public interface IHttpCollectTimeout
    {
        IHttpHaveTimeout Timeout(TimeSpan timeout);
    }

    public interface IHttpCollectRetry
    {
        IHttpHaveRetry Retry(int retry);
    }

    public interface IHttpCollectOnFailure
    {
        IHttpHaveOnFailure OnFailure(Func<HttpStatusCode, Stream, CancellationToken, Task> onFailure);
        IHttpHaveOnFailure OnFailure(Func<HttpResponseMessage, CancellationToken, Task> onFailure);
        IHttpHaveOnFailure OnBadRequest<TError>(Action<TError> onBadRequest);
        IHttpHaveOnFailure OnFailureFromString(Func<HttpStatusCode, string, CancellationToken, Task> onFailure);
    }
}
