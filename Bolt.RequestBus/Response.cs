using System.Collections.Generic;
using System.Linq;

namespace Bolt.RequestBus
{
    public interface IResponse
    {
        bool IsSucceed { get; }
        IEnumerable<IError> Errors { get; }
    }

    public interface IResponse<out TResult> : IResponse
    {
        TResult Value { get; }
    }

    internal sealed class Response : IResponse
    {
        public bool IsSucceed { get; private set; } = false;
        public IEnumerable<IError> Errors { get; private set; } = Enumerable.Empty<IError>();
        
        public static IResponse Succeed() => new Response{ IsSucceed = true };
        public static IResponse Failed(params IError[] errors) => new Response 
            { 
                IsSucceed = false, 
                Errors = errors ?? Enumerable.Empty<IError>() 
            };
        public static IResponse<TResult> Succeed<TResult>(TResult result) => new Response<TResult>{ IsSucceed = true, Value = result };
        public static IResponse<TResult> Failed<TResult>(params IError[] errors) => new Response<TResult>
        {
            IsSucceed = false,
            Errors = errors ?? Enumerable.Empty<IError>(),
            Value = default
        };
    }

    internal sealed class Response<TResult> : IResponse<TResult>
    {
        public bool IsSucceed { get; internal set; } = false;
        public IEnumerable<IError> Errors { get; internal set; } = Enumerable.Empty<IError>();
        public TResult Value { get; internal set; } = default;
    }
}