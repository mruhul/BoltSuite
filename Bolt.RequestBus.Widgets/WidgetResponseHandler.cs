namespace Bolt.RequestBus.Widgets
{
    public abstract class WidgetResponseHandler<TRequest> : IResponseHandler<TRequest, WidgetResponse>
    {
        public abstract Response<WidgetResponse> Handle(IRequestBusContext context, TRequest request);

        public virtual bool IsApplicable(IRequestBusContext context, TRequest request) => true;

        public virtual ExecutionHint ExecutionHint { get; } = ExecutionHint.None;
    }
    
    
    public abstract class WidgetMainResponseHandler<TRequest> : IResponseHandler<TRequest, WidgetResponse>
    {
        public abstract Response<WidgetResponse> Handle(IRequestBusContext context, TRequest request);

        public virtual bool IsApplicable(IRequestBusContext context, TRequest request) => true;

        public ExecutionHint ExecutionHint { get; } = ExecutionHint.Main;
    }

    
    public abstract class WidgetIndependentResponseHandler<TRequest> : IResponseHandler<TRequest, WidgetResponse>
    {
        public abstract Response<WidgetResponse> Handle(IRequestBusContext context, TRequest request);

        public virtual bool IsApplicable(IRequestBusContext context, TRequest request) => true;

        public ExecutionHint ExecutionHint { get; } = ExecutionHint.Independent;
    }
}