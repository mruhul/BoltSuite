namespace Bolt.RequestBus.Widgets
{
    public abstract class WidgetResponseHandler<TRequest> : IResponseHandler<TRequest, IWidgetResponse>
    {
        IResponse<IWidgetResponse> IResponseHandler<TRequest, IWidgetResponse>.Handle(IRequestBusContext context, TRequest request)
        {
            var result = this.Handle(context, request);

            if (StatusCodeHelper.IsSuccessful(result?.StatusCode))
                return Response.Succeed(result);

            return Response.Failed(null, result);
        }

        protected abstract IWidgetResponse Handle(IRequestBusContext context, TRequest request);

        public virtual bool IsApplicable(IRequestBusContext context, TRequest request) => true;

        public virtual ExecutionHint ExecutionHint { get; } = ExecutionHint.None;
    }
    
    
    public abstract class WidgetMainResponseHandler<TRequest> : IResponseHandler<TRequest, IWidgetResponse>
    {
        IResponse<IWidgetResponse> IResponseHandler<TRequest, IWidgetResponse>.Handle(IRequestBusContext context, TRequest request)
        {
            var result = this.Handle(context, request);

            if (StatusCodeHelper.IsSuccessful(result?.StatusCode))
                return Response.Succeed(result);

            return Response.Failed(null, result);
        }

        protected abstract IWidgetResponse Handle(IRequestBusContext context, TRequest request);

        public virtual bool IsApplicable(IRequestBusContext context, TRequest request) => true;

        public ExecutionHint ExecutionHint { get; } = ExecutionHint.Main;
    }

    
    public abstract class WidgetIndependentResponseHandler<TRequest> : IResponseHandler<TRequest, IWidgetResponse>
    {
        IResponse<IWidgetResponse> IResponseHandler<TRequest, IWidgetResponse>.Handle(IRequestBusContext context, TRequest request)
        {
            var result = this.Handle(context, request);

            return Response.Succeed(result);
        }

        protected abstract IWidgetResponse Handle(IRequestBusContext context, TRequest request);

        public virtual bool IsApplicable(IRequestBusContext context, TRequest request) => true;

        public ExecutionHint ExecutionHint { get; } = ExecutionHint.Independent;
    }
}