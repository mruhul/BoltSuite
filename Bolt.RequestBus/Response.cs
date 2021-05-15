using System;

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
        internal const int StatusCodeUnauthorized = 401;
        internal const int StatusCodeInternalServerError = 500;
        internal const int StatusCodeNotFound = 404;
        internal const int StatusCodeFailedDependency = 424;


        public static implicit operator Response(Error value) => Failed(value);
        public static implicit operator Response(Error[] value) => Failed(value);

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

        public static Response NotFound(string statusReason = null) => Failed(statusCode: StatusCodeNotFound, statusReason: statusReason);
        public static Response InternalServerError(string statusReason = null) => Failed(statusCode: StatusCodeInternalServerError, statusReason: statusReason);
        public static Response FailedDependency(string statusReason = null) => Failed(statusCode: StatusCodeFailedDependency, statusReason: statusReason);
        public static Response Unauthorized(string statusReason = null) => Failed(statusCode: StatusCodeUnauthorized, statusReason: statusReason);
        public static Response BadRequest(string statusReason = null, params Error[] errors) => Failed(statusCode: StatusCodeBadRequest, statusReason: statusReason, errors);
        public static Response BadRequest(params Error[] errors) => Failed(statusCode: StatusCodeBadRequest, statusReason: null, errors);

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

    public record Error
    {
        public string Code { get; init; }
        public string Message { get; init; }
        public string PropertyName { get; init; }

        public static Error Create(string message, string propertyName = null, string code = null)
            => new()
            {
                Message = message,
                Code = code,
                PropertyName = propertyName
            };
    }
}