using System.Threading.Tasks;

namespace Bolt.RequestBus.Widgets
{
    public abstract class WidgetResponseHandlerAsync<TRequest> : IResponseHandlerAsync<TRequest, IWidgetResponse>
    {
        async Task<IResponse<IWidgetResponse>> IResponseHandlerAsync<TRequest, IWidgetResponse>.Handle(IRequestBusContext context, TRequest request)
        {
            var result = await this.Handle(context, request);

            return Response.Succeed(result);
        }

        protected abstract Task<IWidgetResponse> Handle(IRequestBusContext context, TRequest request);

        public virtual bool IsApplicable(IRequestBusContext context, TRequest request) => true;

        public virtual ExecutionHint ExecutionHint { get; } = ExecutionHint.None;
    }
    
    
    public abstract class WidgetMainResponseHandlerAsync<TRequest> : IResponseHandlerAsync<TRequest, IWidgetResponse>
    {
        async Task<IResponse<IWidgetResponse>> IResponseHandlerAsync<TRequest, IWidgetResponse>.Handle(IRequestBusContext context, TRequest request)
        {
            var result = await this.Handle(context, request);

            return Response.Succeed(result);
        }

        protected abstract Task<IWidgetResponse> Handle(IRequestBusContext context, TRequest request);

        public virtual bool IsApplicable(IRequestBusContext context, TRequest request) => true;

        public ExecutionHint ExecutionHint { get; } = ExecutionHint.Main;
    }

    
    public abstract class WidgetIndependentResponseHandlerAsync<TRequest> : IResponseHandlerAsync<TRequest, IWidgetResponse>
    {
        async Task<IResponse<IWidgetResponse>> IResponseHandlerAsync<TRequest, IWidgetResponse>.Handle(IRequestBusContext context, TRequest request)
        {
            var result = await this.Handle(context, request);

            return Response.Succeed(result);
        }

        protected abstract Task<IWidgetResponse> Handle(IRequestBusContext context, TRequest request);

        public virtual bool IsApplicable(IRequestBusContext context, TRequest request) => true;

        public ExecutionHint ExecutionHint { get; } = ExecutionHint.Independent;
    }
}