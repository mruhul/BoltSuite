using System.Threading.Tasks;

namespace Bolt.RequestBus
{
    public enum ExecutionHint
    {
        None,
        Independent,
        Main
    }
    
    public interface IResponseHandler<in TRequest, out TResult>
    {
        IResponse<TResult> Handle(IRequestBusContext context, TRequest request);
        bool IsApplicable(IRequestBusContext context, TRequest request);
        ExecutionHint ExecutionHint { get; }
    }
    
    public interface IResponseHandlerAsync<in TRequest, TResult>
    {
        Task<IResponse<TResult>> Handle(IRequestBusContext context, TRequest request);
        bool IsApplicable(IRequestBusContext context, TRequest request);
        ExecutionHint ExecutionHint { get; }
    }
    
    public abstract class ResponseHandler<TResult> : IResponseHandler<None, TResult>
    {
        IResponse<TResult> IResponseHandler<None, TResult>.Handle(IRequestBusContext context, None request)
        {
            var result = this.Handle(context);

            return Response.Succeed(result);
        }

        bool IResponseHandler<None, TResult>.IsApplicable(IRequestBusContext context, None request)
        {
            return this.IsApplicable(context);
        }

        ExecutionHint IResponseHandler<None, TResult>.ExecutionHint => this.ExecutionHint;

        protected abstract TResult Handle(IRequestBusContext context);
        protected virtual bool IsApplicable(IRequestBusContext context) => true;
        protected virtual ExecutionHint ExecutionHint { get; } = ExecutionHint.None;
    }
    
    public abstract class ResponseHandler<TRequest, TResult> : IResponseHandler<TRequest, TResult>
    {
        IResponse<TResult> IResponseHandler<TRequest, TResult>.Handle(IRequestBusContext context, TRequest request)
        {
            var result = this.Handle(context, request);

            return Response.Succeed(result);
        }

        bool IResponseHandler<TRequest, TResult>.IsApplicable(IRequestBusContext context, TRequest request)
        {
            return this.IsApplicable(context, request);
        }
        
        ExecutionHint IResponseHandler<TRequest, TResult>.ExecutionHint => this.ExecutionHint;

        protected abstract TResult Handle(IRequestBusContext context, TRequest request);
        protected virtual bool IsApplicable(IRequestBusContext context, TRequest request) => true;
        protected virtual ExecutionHint ExecutionHint { get; } = ExecutionHint.None;
    }
    
    
    public abstract class ResponseHandlerAsync<TResult> : IResponseHandlerAsync<None, TResult>
    {
        async Task<IResponse<TResult>> IResponseHandlerAsync<None, TResult>.Handle(IRequestBusContext context, None request)
        {
            var result = await this.Handle(context);

            return Response.Succeed(result);
        }

        bool IResponseHandlerAsync<None, TResult>.IsApplicable(IRequestBusContext context, None request)
        {
            return this.IsApplicable(context);
        }

        protected abstract Task<TResult> Handle(IRequestBusContext context);
        protected virtual bool IsApplicable(IRequestBusContext context) => true;
        
        ExecutionHint IResponseHandlerAsync<None, TResult>.ExecutionHint => this.ExecutionHint;
        protected virtual ExecutionHint ExecutionHint { get; } = ExecutionHint.None;
    }
    
    public abstract class ResponseHandlerAsync<TRequest, TResult> : IResponseHandlerAsync<TRequest, TResult>
    {
        async Task<IResponse<TResult>> IResponseHandlerAsync<TRequest, TResult>.Handle(IRequestBusContext context, TRequest request)
        {
            var result = await this.Handle(context, request);

            return Response.Succeed(result);
        }

        bool IResponseHandlerAsync<TRequest, TResult>.IsApplicable(IRequestBusContext context, TRequest request)
        {
            return this.IsApplicable(context, request);
        }

        protected abstract Task<TResult> Handle(IRequestBusContext context, TRequest request);
        protected virtual bool IsApplicable(IRequestBusContext context, TRequest request) => true;
        
        ExecutionHint IResponseHandlerAsync<TRequest, TResult>.ExecutionHint => this.ExecutionHint;
        protected virtual ExecutionHint ExecutionHint { get; } = ExecutionHint.None;
    }
}