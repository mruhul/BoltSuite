using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bolt.RequestBus
{
    public interface IResponseFilter<in TRequest,TResult>
    {
        void Filter(IRequestBusContext context, TRequest request,
            ICollection<IResponse<TResult>> currentResult);

        bool IsApplicable(IRequestBusContext context, TRequest request);
    }
    
    public interface IResponseFilterAsync<in TRequest,TResult>
    {
        Task Filter(IRequestBusContext context, TRequest request,
            ICollection<IResponse<TResult>> currentResult);

        bool IsApplicable(IRequestBusContext context, TRequest request);
    }

    public abstract class ResponseFilter<TResult> : IResponseFilter<None, TResult>
    {
        void IResponseFilter<None, TResult>.Filter(IRequestBusContext context, None request, ICollection<IResponse<TResult>> currentResult)
        {
            this.Filter(context, currentResult);
        }

        bool IResponseFilter<None, TResult>.IsApplicable(IRequestBusContext context, None request)
        {
            return this.IsApplicable(context);
        }

        protected abstract void Filter(IRequestBusContext context,
            ICollection<IResponse<TResult>> currentResult);

        protected virtual bool IsApplicable(IRequestBusContext context) => true;
    }
    
    public abstract class ResponseFilter<TRequest, TResult> : IResponseFilter<TRequest, TResult>
    {
        void IResponseFilter<TRequest, TResult>.Filter(IRequestBusContext context, TRequest request, ICollection<IResponse<TResult>> currentResult)
        {
            this.Filter(context, request, currentResult);
        }
        protected abstract void Filter(IRequestBusContext context, TRequest request,
            ICollection<IResponse<TResult>> currentResult);

        public virtual bool IsApplicable(IRequestBusContext context, TRequest request) => true;
    }
    
    public abstract class ResponseFilterAsync<TResult> : IResponseFilterAsync<None, TResult>
    {
        Task IResponseFilterAsync<None, TResult>.Filter(IRequestBusContext context, None request, ICollection<IResponse<TResult>> currentResult)
        {
            return this.Filter(context, currentResult);
        }

        bool IResponseFilterAsync<None, TResult>.IsApplicable(IRequestBusContext context, None request)
        {
            return this.IsApplicable(context);
        }

        protected abstract Task Filter(IRequestBusContext context,
            ICollection<IResponse<TResult>> currentResult);

        protected virtual bool IsApplicable(IRequestBusContext context) => true;
    }
    
    public abstract class ResponseFilterAsync<TRequest, TResult> : IResponseFilterAsync<TRequest, TResult>
    {
        Task IResponseFilterAsync<TRequest, TResult>.Filter(IRequestBusContext context, TRequest request, ICollection<IResponse<TResult>> currentResult)
        {
            return this.Filter(context, request, currentResult);
        }

        protected abstract Task Filter(IRequestBusContext context, TRequest request,
            ICollection<IResponse<TResult>> currentResult);

        public virtual bool IsApplicable(IRequestBusContext context, TRequest request) => true;
    }
}