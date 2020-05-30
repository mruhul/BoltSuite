using System.Threading.Tasks;

namespace Bolt.RequestBus
{
    public interface IRequestHandler<in TRequest, out TResult>
    {
        IResponse<TResult> Handle(IRequestBusContext context, TRequest request);
        bool IsApplicable(IRequestBusContext context, TRequest request);
    }
    
    public interface IRequestHandlerAsync<in TRequest, TResult>
    {
        Task<IResponse<TResult>> Handle(IRequestBusContext context, TRequest request);
        bool IsApplicable(IRequestBusContext context, TRequest request);
    }
    
    public abstract class RequestHandler<TRequest> : IRequestHandler<TRequest, None>
    {
        IResponse<None> IRequestHandler<TRequest, None>.Handle(IRequestBusContext context, TRequest request)
        {
            this.Handle(context, request);

            return Response.Succeed(None.Instance);
        }

        public virtual bool IsApplicable(IRequestBusContext context, TRequest request) => true;
        
        protected abstract void Handle(IRequestBusContext context, TRequest request);
    }
    
    public abstract class RequestHandlerAsync<TRequest> : IRequestHandlerAsync<TRequest, None>
    {
        async Task<IResponse<None>> IRequestHandlerAsync<TRequest, None>.Handle(IRequestBusContext context, TRequest request)
        {
            await this.Handle(context, request);

            return Response.Succeed(None.Instance);
        }

        public virtual bool IsApplicable(IRequestBusContext context, TRequest request) => true;
        
        protected abstract Task Handle(IRequestBusContext context, TRequest request);
    }
    
    public abstract class RequestHandler<TRequest, TResult> : IRequestHandler<TRequest, TResult>
    {
        IResponse<TResult> IRequestHandler<TRequest, TResult>.Handle(IRequestBusContext context, TRequest request)
        {
            var result = this.Handle(context, request);

            return Response.Succeed(result);
        }

        public virtual bool IsApplicable(IRequestBusContext context, TRequest request) => true;
        
        protected abstract TResult Handle(IRequestBusContext context, TRequest request);
    }
    
    public abstract class RequestHandlerAsync<TRequest, TResult> : IRequestHandlerAsync<TRequest, TResult>
    {
        async Task<IResponse<TResult>> IRequestHandlerAsync<TRequest, TResult>.Handle(IRequestBusContext context, TRequest request)
        {
            var result = await this.Handle(context, request);

            return Response.Succeed(result);
        }

        public virtual bool IsApplicable(IRequestBusContext context, TRequest request) => true;
        
        protected abstract Task<TResult> Handle(IRequestBusContext context, TRequest request);
    }
}