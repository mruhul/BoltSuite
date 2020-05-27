using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bolt.RequestBus
{
    public interface IRequestBus
    {
        IResponse Send<TRequest>(TRequest request);
        IResponse TrySend<TRequest>(TRequest request);
        
        IResponse<TResult> Send<TRequest,TResult>(TRequest request);
        IResponse<TResult> TrySend<TRequest,TResult>(TRequest request);
        
        void Publish<TEvent>(TEvent @event);
        
        Task<IResponse> SendAsync<TRequest>(TRequest request);
        Task<IResponse> TrySendAsync<TRequest>(TRequest request);
        
        Task<IResponse<TResult>> SendAsync<TRequest,TResult>(TRequest request);
        Task<IResponse<TResult>> TrySendAsync<TRequest,TResult>(TRequest request);
        
        Task PublishAsync<TEvent>(TEvent @event);

        IResponse<TResult> Response<TResult>();
        IEnumerable<IResponse<TResult>> Responses<TResult>();
        IEnumerable<IResponse<TResult>> Responses<TRequest,TResult>(TRequest request);
        
        Task<IResponse<TResult>> ResponseAsync<TResult>();
        Task<IEnumerable<IResponse<TResult>>> ResponsesAsync<TResult>();
        Task<IEnumerable<IResponse<TResult>>> ResponsesAsync<TRequest,TResult>(TRequest request);
    }

    public interface IResponse
    {
        bool IsSucceed { get; }
        IEnumerable<IError> Errors { get; }
    }

    public interface IResponse<out TResult> : IResponse
    {
        TResult Value { get; }
    }

    public interface IError
    {
        string Code { get; }
        string Message { get; }
        string PropertyName { get; }
    }
}