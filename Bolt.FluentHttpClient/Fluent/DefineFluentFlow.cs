using System;
using System.Collections.Generic;
using System.Text;

namespace Bolt.FluentHttpClient.Fluent
{
    public interface IHttpHaveClient : IHttpCollectUrl { }

    public interface IHttpHaveTimeout :
        IHttpCollectRetry,
        IHttpCollectOnFailure,
        IHttpSendRequest,
        IHttpSendTypedRequest
    { }

    public interface IHttpHaveRetry :
        IHttpCollectOnFailure,
        IHttpSendRequest,
        IHttpSendTypedRequest
    { }

    public interface IHttpHaveHeaders :
        IHttpCollectHeaders,
        IHttpCollectTimeout,
        IHttpCollectRetry,
        IHttpCollectOnFailure,
        IHttpSendRequest,
        IHttpSendTypedRequest
    { }

    public interface IHttpHaveUrl :
        IHttpCollectQueryStrings,
        IHttpCollectHeaders,
        IHttpCollectTimeout,
        IHttpCollectRetry,
        IHttpCollectOnFailure,
        IHttpSendRequest,
        IHttpSendTypedRequest
    { }

    public interface IHttpHaveQueryStrings :
        IHttpCollectQueryStrings,
        IHttpCollectHeaders,
        IHttpCollectTimeout,
        IHttpCollectRetry,
        IHttpCollectOnFailure,
        IHttpSendRequest,
        IHttpSendTypedRequest
    { }

    public interface IHttpHaveOnFailure
        : IHttpSendTypedRequest
    {
    }
}
