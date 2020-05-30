using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bolt.RequestBus
{
    public interface IRequestValidator<in TRequest>
    {
        IEnumerable<IError> Validate(IRequestBusContext context, TRequest request);
        bool IsApplicable(IRequestBusContext context, TRequest request);
        /// <summary>
        /// High priority value will execute first in order
        /// </summary>
        int Priority { get; }
    }
    
    public interface IRequestValidatorAsync<in TRequest>
    {
        Task<IEnumerable<IError>> Validate(IRequestBusContext context, TRequest request);
        bool IsApplicable(IRequestBusContext context, TRequest request);
        /// <summary>
        /// High priority value will execute first in order
        /// </summary>
        int Priority { get; }
    }
    
    public abstract class RequestValidator<TRequest> : IRequestValidator<TRequest>
    {
        public abstract IEnumerable<IError> Validate(IRequestBusContext context, TRequest request);

        public virtual bool IsApplicable(IRequestBusContext context, TRequest request) => true;
        public virtual int Priority { get; } = 0;
    }
    
    public abstract class RequestValidatorAsync<TRequest> : IRequestValidatorAsync<TRequest>
    {
        public abstract Task<IEnumerable<IError>> Validate(IRequestBusContext context, TRequest request);

        public virtual bool IsApplicable(IRequestBusContext context, TRequest request) => true;
        
        public virtual int Priority { get; } = 0;
    }
}