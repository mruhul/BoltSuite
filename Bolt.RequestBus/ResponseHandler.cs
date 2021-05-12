using System.Threading.Tasks;

namespace Bolt.RequestBus
{
    public enum ExecutionHint
    {
        None,
        Independent,
        Main
    }
    
    public interface IResponseHandler<in TRequest, TResult>
    {
        Response<TResult> Handle(IRequestBusContext context, TRequest request);
        bool IsApplicable(IRequestBusContext context, TRequest request);
        ExecutionHint ExecutionHint { get; }
    }
    
    public interface IResponseHandlerAsync<in TRequest, TResult>
    {
        Task<Response<TResult>> Handle(IRequestBusContext context, TRequest request);
        bool IsApplicable(IRequestBusContext context, TRequest request);
        ExecutionHint ExecutionHint { get; }
    }
    
    public abstract class ResponseHandler<TResult> : IResponseHandler<None, TResult>
    {
        public abstract Response<TResult> Handle(IRequestBusContext context, None request);

        bool IResponseHandler<None, TResult>.IsApplicable(IRequestBusContext context, None request)
        {
            return this.IsApplicable(context);
        }

        protected virtual bool IsApplicable(IRequestBusContext context) => true;

        public virtual ExecutionHint ExecutionHint => ExecutionHint.None;
    }
    
    public abstract class ResponseHandler<TRequest, TResult> : IResponseHandler<TRequest, TResult>
    {
        public abstract Response<TResult> Handle(IRequestBusContext context, TRequest request);
        
        public virtual bool IsApplicable(IRequestBusContext context, TRequest request) => true;
        
        public virtual ExecutionHint ExecutionHint => ExecutionHint.None;

    }
    
    public abstract class ResponseHandlerAsync<TResult> : IResponseHandlerAsync<None, TResult>
    {
        async Task<Response<TResult>> IResponseHandlerAsync<None, TResult>.Handle(IRequestBusContext context, None request)
        {
            return await this.Handle(context);
        }

        bool IResponseHandlerAsync<None, TResult>.IsApplicable(IRequestBusContext context, None request)
            => this.IsApplicable(context);

        protected virtual bool IsApplicable(IRequestBusContext context) => true;
        public virtual ExecutionHint ExecutionHint => ExecutionHint.None;
        protected abstract Task<Response<TResult>> Handle(IRequestBusContext context);
    }
    
    public abstract class ResponseHandlerAsync<TRequest, TResult> : IResponseHandlerAsync<TRequest, TResult>
    {
        public virtual bool IsApplicable(IRequestBusContext context, TRequest request) => true;

        public virtual ExecutionHint ExecutionHint => ExecutionHint.None;

        public abstract Task<Response<TResult>> Handle(IRequestBusContext context, TRequest request);
    }
}