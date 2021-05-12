using System.Threading.Tasks;

namespace Bolt.RequestBus
{
    public interface IRequestHandler<in TRequest, TResult>
    {
        Response<TResult> Handle(IRequestBusContext context, TRequest request);
        bool IsApplicable(IRequestBusContext context, TRequest request);
    }
    
    public interface IRequestHandlerAsync<in TRequest, TResult>
    {
        Task<Response<TResult>> Handle(IRequestBusContext context, TRequest request);
        bool IsApplicable(IRequestBusContext context, TRequest request);
    }
    
    public abstract class RequestHandler<TRequest> : IRequestHandler<TRequest, None>
    {
        Response<None> IRequestHandler<TRequest, None>.Handle(IRequestBusContext context, TRequest request)
        {
            var rsp = this.Handle(context, request);

            return new Response<None>
            {
                Errors = rsp.Errors,
                IsSucceed = rsp.IsSucceed,
                StatusCode = rsp.StatusCode
            };
        }

        public virtual bool IsApplicable(IRequestBusContext context, TRequest request) => true;
        
        protected abstract Response Handle(IRequestBusContext context, TRequest request);
    }
    
    public abstract class RequestHandlerAsync<TRequest> : IRequestHandlerAsync<TRequest, None>
    {
        async Task<Response<None>> IRequestHandlerAsync<TRequest, None>.Handle(IRequestBusContext context, TRequest request)
        {
            var rsp = await this.Handle(context, request);

            return new Response<None>
            {
                StatusCode = rsp.StatusCode,
                IsSucceed = rsp.IsSucceed,
                Errors = rsp.Errors
            };
        }

        public virtual bool IsApplicable(IRequestBusContext context, TRequest request) => true;
        
        protected abstract Task<Response> Handle(IRequestBusContext context, TRequest request);
    }
    
    public abstract class RequestHandler<TRequest, TResult> : IRequestHandler<TRequest, TResult>
    {
        public virtual bool IsApplicable(IRequestBusContext context, TRequest request) => true;
        
        public abstract Response<TResult> Handle(IRequestBusContext context, TRequest request);
    }
    
    public abstract class RequestHandlerAsync<TRequest, TResult> : IRequestHandlerAsync<TRequest, TResult>
    {
        public abstract Task<Response<TResult>> Handle(IRequestBusContext context, TRequest request);

        public virtual bool IsApplicable(IRequestBusContext context, TRequest request) => true;
    }
}