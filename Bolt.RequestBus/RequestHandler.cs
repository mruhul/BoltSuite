namespace Bolt.RequestBus
{
    public interface IRequestHandler<in TRequest, out TResult>
    {
        public IResponse<TResult> Handle(IRequestBusContext context, TRequest request);
        public bool IsApplicable(IRequestBusContext context, TRequest request);
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
}