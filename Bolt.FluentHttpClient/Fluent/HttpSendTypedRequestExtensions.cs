using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Bolt.FluentHttpClient.Fluent
{
    public static class HttpSendTypedRequestExtensions
    {
        /// <summary>
        /// Send an http GET request and return a response with statuscode. 
        /// Use this if the get request doesn't return any response
        /// or you don't care about the response content but statuscode. 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static Task<IHttpResponse> GetAsync(this IHttpSendTypedRequest source, CancellationToken cancellationToken = default)
        {
            return source.SendAsync(HttpMethod.Get, cancellationToken);
        }

        /// <summary>
        /// Send an http GET request and return a response with statuse code 
        /// and deserialized TContent if the response is successful.
        /// </summary>
        /// <typeparam name="TContent"></typeparam>
        /// <param name="source"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static Task<IHttpResponse<TContent>> GetAsync<TContent>(this IHttpSendTypedRequest source, CancellationToken cancellationToken = default)
        {
            return source.SendAsync<TContent>(HttpMethod.Get, cancellationToken);
        }

        /// <summary>
        /// Send a http DELETE request and return response with statuscode
        /// </summary>
        /// <param name="source"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static Task<IHttpResponse> DeleteAsync(this IHttpSendTypedRequest source, CancellationToken cancellationToken = default)
        {
            return source.SendAsync(HttpMethod.Delete, cancellationToken);
        }

        /// <summary>
        /// Send a http DELETE request and return response with statuscode and TContent which deserialized from http rresponse content
        /// </summary>
        /// <typeparam name="TContent"></typeparam>
        /// <param name="source"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static Task<IHttpResponse<TContent>> DeleteAsync<TContent>(this IHttpSendTypedRequest source, CancellationToken cancellationToken = default)
        {
            return source.SendAsync<TContent>(HttpMethod.Delete, cancellationToken);
        }

        #region post

        public static Task<IHttpResponse> PostAsync(this IHttpSendTypedRequest source, CancellationToken cancellationToken = default)
        {
            return source.SendAsync(HttpMethod.Post, cancellationToken);
        }

        public static Task<IHttpResponse> PostAsync<TInput>(this IHttpSendTypedRequest source, TInput input, string contentType, CancellationToken cancellationToken = default)
        {
            return source.SendAsync(HttpMethod.Post, input, contentType, cancellationToken);
        }

        public static Task<IHttpResponse> PostAsync<TInput>(this IHttpSendTypedRequest source, TInput input, CancellationToken cancellationToken = default)
        {
            return source.SendAsync(HttpMethod.Post, input, Constants.ContentTypeJson, cancellationToken);
        }

        public static Task<IHttpResponse<TContent>> PostAsync<TContent>(this IHttpSendTypedRequest source, CancellationToken cancellationToken = default)
        {
            return source.SendAsync<TContent>(HttpMethod.Post, cancellationToken);
        }

        public static Task<IHttpResponse<TContent>> PostAsync<TInput,TContent>(this IHttpSendTypedRequest source, TInput input, string contentType, CancellationToken cancellationToken = default)
        {
            return source.SendAsync<TInput,TContent>(HttpMethod.Post, input, contentType, cancellationToken);
        }

        public static Task<IHttpResponse<TContent>> PostAsync<TInput, TContent>(this IHttpSendTypedRequest source, TInput input, CancellationToken cancellationToken = default)
        {
            return source.SendAsync<TInput, TContent>(HttpMethod.Post, input, Constants.ContentTypeJson, cancellationToken);
        }

        #endregion


        #region put

        public static Task<IHttpResponse> PutAsync<TInput>(this IHttpSendTypedRequest source, TInput input, string contentType, CancellationToken cancellationToken)
        {
            return source.SendAsync(HttpMethod.Put, input, contentType, cancellationToken);
        }

        public static Task<IHttpResponse> PutAsync<TInput>(this IHttpSendTypedRequest source, TInput input, CancellationToken cancellationToken)
        {
            return source.SendAsync(HttpMethod.Put, input, Constants.ContentTypeJson, cancellationToken);
        }

        public static Task<IHttpResponse<TContent>> PutAsync<TContent>(this IHttpSendTypedRequest source, CancellationToken cancellationToken)
        {
            return source.SendAsync<TContent>(HttpMethod.Put, cancellationToken);
        }

        public static Task<IHttpResponse<TContent>> PutAsync<TInput, TContent>(this IHttpSendTypedRequest source, TInput input, string contentType, CancellationToken cancellationToken)
        {
            return source.SendAsync<TInput, TContent>(HttpMethod.Put, input, contentType, cancellationToken);
        }

        public static Task<IHttpResponse<TContent>> PutAsync<TInput, TContent>(this IHttpSendTypedRequest source, TInput input, CancellationToken cancellationToken)
        {
            return source.SendAsync<TInput, TContent>(HttpMethod.Put, input, Constants.ContentTypeJson, cancellationToken);
        }

        #endregion
    }
}
