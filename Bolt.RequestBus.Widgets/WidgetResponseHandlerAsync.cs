using System.Threading.Tasks;

namespace Bolt.RequestBus.Widgets
{
    public abstract class WidgetResponseHandlerAsync<TRequest> : IResponseHandlerAsync<TRequest, WidgetResponse>
    {
        public abstract Task<Response<WidgetResponse>> Handle(IRequestBusContext context, TRequest request);

        public virtual bool IsApplicable(IRequestBusContext context, TRequest request) => true;

        public virtual ExecutionHint ExecutionHint { get; } = ExecutionHint.None;
    }
    
    
    public abstract class WidgetMainResponseHandlerAsync<TRequest> : IResponseHandlerAsync<TRequest, WidgetResponse>
    {
        public abstract Task<Response<WidgetResponse>> Handle(IRequestBusContext context, TRequest request);

        public virtual bool IsApplicable(IRequestBusContext context, TRequest request) => true;

        public ExecutionHint ExecutionHint { get; } = ExecutionHint.Main;
    }

    
    public abstract class WidgetIndependentResponseHandlerAsync<TRequest> : IResponseHandlerAsync<TRequest, WidgetResponse>
    {
        public abstract Task<Response<WidgetResponse>> Handle(IRequestBusContext context, TRequest request);

        public virtual bool IsApplicable(IRequestBusContext context, TRequest request) => true;

        public ExecutionHint ExecutionHint { get; } = ExecutionHint.Independent;
    }
}