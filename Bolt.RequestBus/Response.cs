using System;
using System.Net;

namespace Bolt.RequestBus
{
    public record Response
    {
        public int? StatusCode { get; init; }
        public string StatusReason { get; init; }
        public bool IsSucceed { get; init; }
        public Error[] Errors { get; init; } = Array.Empty<Error>();

        internal const int StatusCodeOkay = 200;
        internal const int StatusCodeBadRequest = 400;


        public static implicit operator Response(Error value) => Failed(value);
        public static implicit operator Response(Error[] value) => Failed(value);
        public static implicit operator Response(HttpError error)
        {
            var statusCode = (int)error.StatusCode;

            return new Response
            {
                IsSucceed = statusCode is >= 200 and <= 299,
                StatusCode = statusCode,
                StatusReason = error.StatusReason
            };
        }

        public static Response Ok()
            => new()
            {
                IsSucceed = true,
                StatusCode = StatusCodeOkay,
            };

        public static Response Ok(int statusCode)
            => new()
            {
                IsSucceed = true,
                StatusCode = statusCode
            };

        public static Response<TValue> Ok<TValue>(TValue value)
            => new()
            {
                IsSucceed = true,
                StatusCode = StatusCodeOkay,
                Value = value
            };

        public static Response<TValue> Ok<TValue>(int statusCode, TValue value)
            => new()
            {
                IsSucceed = true,
                StatusCode = statusCode,
                Value = value
            };

        public static Response Failed(params Error[] errors)
            => new()
            {
                IsSucceed = false,
                Errors = errors,
                StatusCode = StatusCodeBadRequest
            };

        public static Response<TValue> Failed<TValue>(params Error[] errors)
            => new()
            {
                IsSucceed = false,
                Errors = errors,
                StatusCode = StatusCodeBadRequest
            };
        
        private static Response Failed(int statusCode, string statusReason = null, params Error[] errors) => new()
        {
            StatusReason = statusReason,
            IsSucceed = false,
            StatusCode = statusCode,
            Errors = errors ?? Array.Empty<Error>()
        };

        private static Response<TValue> Failed<TValue>(int statusCode, string statusReason = null, params Error[] errors) => new()
        {
            StatusReason = statusReason,
            IsSucceed = false,
            StatusCode = statusCode,
            Errors = errors ?? Array.Empty<Error>()
        };

        public static Response<TValue> Failed<TValue>(int statusCode, string statusReason = null, TValue value = default, params Error[] errors) => new()
        {
            StatusReason = statusReason,
            IsSucceed = false,
            StatusCode = statusCode,
            Errors = errors ?? Array.Empty<Error>(),
            Value = value
        };
    }

    public record Response<TValue>
    {
        public int? StatusCode { get; init; }
        public string StatusReason { get; init; }
        public bool IsSucceed { get; init; }
        public Error[] Errors { get; init; } = Array.Empty<Error>();
        public TValue Value { get; init; }


        public static implicit operator Response<TValue>(TValue value) => Response.Ok(value);

        public static implicit operator Response<TValue>(HttpError error)
        {
            var statusCode = (int)error.StatusCode;

            return new Response<TValue>
            {
                IsSucceed = statusCode is >= 200 and <= 299,
                StatusCode = statusCode,
                StatusReason = error.StatusReason
            };
        }

        public static implicit operator Response<TValue>(Error value) => Response.Failed<TValue>(Response.StatusCodeBadRequest, null, default, value);
        public static implicit operator Response<TValue>(Error[] value) => Response.Failed<TValue>(Response.StatusCodeBadRequest, null, default, value);
        public static implicit operator Response<TValue>(Response response) => new()
        {
            Errors = response.Errors,
            IsSucceed = response.IsSucceed,
            StatusCode = response.StatusCode,
            StatusReason = response.StatusReason
        };
        public static implicit operator Response(Response<TValue> response) => new()
        {
            Errors = response.Errors,
            IsSucceed = response.IsSucceed,
            StatusCode = response.StatusCode,
            StatusReason = response.StatusReason
        };
    }

    /// <summary>
    /// Purpose of this class is to easily return failure of response or response`T. This class instance will implicitly convert to Response or Response`T
    /// </summary>
    public record HttpError
    {
        private HttpError() { }

        public HttpStatusCode StatusCode { get; init; }
        public string StatusReason { get; init; }
        
        public static HttpError NotFound(string statusReason = "Resource not found") => new()
        {
            StatusReason = statusReason,
            StatusCode = HttpStatusCode.NotFound
        };

        public static HttpError InternalServerError(string statusReason = "Unexpected error occurred") => new()
        {
            StatusReason = statusReason,
            StatusCode = HttpStatusCode.InternalServerError
        };

        public static HttpError FailedDependency(string statusReason = "Dependency failed") => new()
        {
            StatusReason = statusReason,
            StatusCode = HttpStatusCode.FailedDependency
        };

        public static HttpError Unauthorized(string statusReason = "User not authorized to perform the request") => new()
        {
            StatusReason = statusReason,
            StatusCode = HttpStatusCode.Unauthorized
        };

        public static HttpError Forbidden(string statusReason = "Request is forbidden") => new()
        {
            StatusReason = statusReason,
            StatusCode = HttpStatusCode.Forbidden
        };

        public static HttpError Locked(string statusReason = "Resource is locked") => new()
        {
            StatusReason = statusReason,
            StatusCode = HttpStatusCode.Locked
        };

        public static HttpError PaymentRequired(string statusReason = "Payment required") => new()
        {
            StatusReason = statusReason,
            StatusCode = HttpStatusCode.PaymentRequired
        };

        /// <summary>
        /// Create new instance of HttpError
        /// </summary>
        /// <param name="statusCode">Only non successful status code allowed</param>
        /// <param name="statusReason"></param>
        /// <returns></returns>
        /// <exception cref="Exception">Return exception when successful status code provided.</exception>
        public static HttpError New(HttpStatusCode statusCode, string statusReason)
        {
            var result = new HttpError {StatusCode = statusCode, StatusReason = statusReason};
            if (IsSuccessfulStatusCode(statusCode)) throw new Exception("HttpError only allow non successful status code.");
            return result;
        }

        private static bool IsSuccessfulStatusCode(HttpStatusCode statusCode) => (int)statusCode is >= 200 and <= 299;
    }
}