using System;

namespace Bolt.RequestBus
{
    public record Response
    {
        public int? StatusCode { get; init; }
        public string StatusReason { get; init; }
        public bool IsSucceed { get; init; }
        public Error[] Errors { get; init; } = Array.Empty<Error>();

        private const int StatusCodeOkay = 200;
        private const int StatusCodeBadRequest = 400;
        private const int StatusCodeInternalServerError = 500;

        public static implicit operator Response(bool value) => value ? Ok() : Failed();
        public static implicit operator bool(Response rsp) => rsp?.IsSucceed ?? false;

        public static implicit operator Response(Error value) => Failed(value);
        public static implicit operator Response(Error[] value) => Failed(value);
        public static implicit operator Response(ResponseStatus value) => new()
        {
            Errors = value.Errors,
            IsSucceed = value.StatusCode is >= 200 and <= 299,
            StatusCode = value.StatusCode,
            StatusReason = value.StatusReason
        };

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
                StatusCode = statusCode,
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

        public static Response<TValue> Failed<TValue>(Error[] errors, TValue value)
            => new()
            {
                IsSucceed = false,
                Errors = errors,
                StatusCode = StatusCodeBadRequest,
                Value = value
            };

        public static Response<TValue> Failed<TValue>(int statusCode, TValue value = default)
            => new()
            {
                IsSucceed = false,
                StatusCode = statusCode,
                Value = value
            };

        public static Response<TValue> Failed<TValue>(TValue value = default)
            => new()
            {
                IsSucceed = false,
                StatusCode = StatusCodeInternalServerError,
                Value = value
            };
    }

    public record Response<TValue> : Response
    {
        public TValue Value { get; init; }


        public static implicit operator Response<TValue>(TValue value) => Ok(value);
        public static implicit operator Response<TValue>(Error value) => Failed<TValue>(value);
        public static implicit operator Response<TValue>(Error[] value) => Failed<TValue>(value);
        public static implicit operator Response<TValue>(ResponseStatus value) => new()
        {
            Errors = value.Errors,
            IsSucceed = value.StatusCode is >= 200 and <= 299,
            StatusCode = value.StatusCode,
            StatusReason = value.StatusReason
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

    public record ResponseStatus
    {
        public int StatusCode { get; init; }
        public string StatusReason { get; init; }
        public Error[] Errors { get; init; } = Array.Empty<Error>();

        public static ResponseStatus BadRequest(params Error[] errors) 
            => BadRequest(null, errors);

        public static ResponseStatus BadRequest(string statusReason, params Error[] errors) 
            => New (400, statusReason, errors);

        public static ResponseStatus NotFound(string statusReason = null) 
            => New(404, statusReason);

        public static ResponseStatus InternalServerError(string statusReason = null)
            => New(500, statusReason);

        public static ResponseStatus FailedDependency(string statusReason = null)
            => New(424, statusReason);

        public static ResponseStatus Unauthorized(string statusReason = null)
            => New(401, statusReason);

        public static ResponseStatus New(int statusCode, string statusReason, params Error[] errors) => new()
        {
            StatusReason = statusReason,
            StatusCode = statusCode,
            Errors = errors ?? Array.Empty<Error>()
        };
    }
}