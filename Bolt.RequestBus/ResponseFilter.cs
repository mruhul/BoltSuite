using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bolt.RequestBus
{
    public interface IResponseFilter<in TRequest, TResult>
    {
        void Filter(IRequestBusContext context, TRequest request, ResponseCollection<TResult> rspCollection);

        bool IsApplicable(IRequestBusContext context, TRequest request);
    }
    
    public interface IResponseFilterAsync<in TRequest, TResult>
    {
        Task Filter(IRequestBusContext context, TRequest request,
            ResponseCollection<TResult> rspCollection);

        bool IsApplicable(IRequestBusContext context, TRequest request);
    }

    public abstract class ResponseFilter<TResult> : IResponseFilter<None, TResult>
    {
        void IResponseFilter<None, TResult>.Filter(IRequestBusContext context, None request, ResponseCollection<TResult> rspCollection)
        {
            this.Filter(context, rspCollection);
        }

        bool IResponseFilter<None, TResult>.IsApplicable(IRequestBusContext context, None request)
        {
            return this.IsApplicable(context);
        }

        protected abstract void Filter(IRequestBusContext context, ResponseCollection<TResult> rspCollection);

        protected virtual bool IsApplicable(IRequestBusContext context) => true;
    }
    
    public abstract class ResponseFilter<TRequest, TResult> : IResponseFilter<TRequest, TResult>
    {
        void IResponseFilter<TRequest, TResult>.Filter(IRequestBusContext context, TRequest request, ResponseCollection<TResult> rspCollection)
        {
            this.Filter(context, request, rspCollection);
        }
        protected abstract void Filter(IRequestBusContext context, TRequest request, ResponseCollection<TResult> rspCollection);

        public virtual bool IsApplicable(IRequestBusContext context, TRequest request) => true;
    }
    
    public abstract class ResponseFilterAsync<TResult> : IResponseFilterAsync<None, TResult>
    {
        Task IResponseFilterAsync<None, TResult>.Filter(IRequestBusContext context, None request, ResponseCollection<TResult> rspCollection)
        {
            return this.Filter(context, rspCollection);
        }

        bool IResponseFilterAsync<None, TResult>.IsApplicable(IRequestBusContext context, None request)
        {
            return this.IsApplicable(context);
        }

        protected abstract Task Filter(IRequestBusContext context, ResponseCollection<TResult> rspCollection);

        protected virtual bool IsApplicable(IRequestBusContext context) => true;
    }
    
    public abstract class ResponseFilterAsync<TRequest, TResult> : IResponseFilterAsync<TRequest, TResult>
    {
        Task IResponseFilterAsync<TRequest, TResult>.Filter(IRequestBusContext context, TRequest request, ResponseCollection<TResult> rspCollection)
        {
            return this.Filter(context, request, rspCollection);
        }

        protected abstract Task Filter(IRequestBusContext context, TRequest request, ResponseCollection<TResult> rspCollection);

        public virtual bool IsApplicable(IRequestBusContext context, TRequest request) => true;
    }
}