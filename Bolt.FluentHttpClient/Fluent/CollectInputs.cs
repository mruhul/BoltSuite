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
        /// <summary>
        /// Add a header with name and value
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        IHttpHaveHeaders Header(string name, string value);

        /// <summary>
        /// Add a collection of headers with name value unit
        /// </summary>
        /// <param name="headers"></param>
        /// <returns></returns>
        IHttpHaveHeaders Headers(NameValueUnit[] headers);
    }

    public interface IHttpCollectTimeout
    {
        /// <summary>
        /// Set the maximum time you like to wait to get a response on http request
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns></returns>
        IHttpHaveTimeout Timeout(TimeSpan timeout);
    }

    public interface IHttpCollectRetry
    {
        /// <summary>
        /// Set how many times you like to retry. Retry will happen only for transient failures eg. RequestTimeout/InternalServerError
        /// </summary>
        /// <param name="retry"></param>
        /// <returns></returns>
        IHttpHaveRetry Retry(int retry);
    }

    public interface IHttpCollectOnFailure
    {
        /// <summary>
        /// Pass a func if you like to handle failure to read errors from stream based on status code
        /// </summary>
        /// <param name="onFailure"></param>
        /// <returns></returns>
        IHttpHaveOnFailure OnFailure(Func<HttpStatusCode, Stream, CancellationToken, Task> onFailure);
        /// <summary>
        /// Pass a func if you like to handle failure based on <see cref="HttpResponseMessage"/>
        /// </summary>
        /// <param name="onFailure"></param>
        /// <returns></returns>
        IHttpHaveOnFailure OnFailure(Func<HttpResponseMessage, CancellationToken, Task> onFailure);
        /// <summary>
        /// Pass an action if you want to read error of type TError when response got bad request
        /// </summary>
        /// <typeparam name="TError"></typeparam>
        /// <param name="onBadRequest"></param>
        /// <returns></returns>
        IHttpHaveOnFailure OnBadRequest<TError>(Action<TError> onBadRequest);
        /// <summary>
        /// Pass a func if you want to get the response as string when failure happen
        /// </summary>
        /// <param name="onFailure"></param>
        /// <returns></returns>
        IHttpHaveOnFailure OnFailureFromString(Func<HttpStatusCode, string, CancellationToken, Task> onFailure);
    }
}
