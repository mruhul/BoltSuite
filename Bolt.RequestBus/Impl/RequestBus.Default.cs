using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Bolt.RequestBus.Impl
{
    internal sealed partial class RequestBus : IRequestBus
    {
        private readonly IServiceProvider _sp;
        private readonly ILogger<RequestBus> _logger;
        private readonly Lazy<IRequestBusContext> _context;

        public RequestBus(IServiceProvider sp, ILogger<RequestBus> logger)
        {
            _sp = sp;
            _logger = logger;
            _context = new Lazy<IRequestBusContext>(() => BuildContext(_sp));
        }


        private static IRequestBusContext BuildContext(IServiceProvider sp)
        {
            var context = new RequestBusContext();
            var writers = sp.GetServices<IRequestBusContextWriter>();

            foreach (var writer in writers)
            {
                writer.Write(context);
            }

            return context;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Response<TValue> Validate<TRequest, TValue>(IRequestBusContext context, TRequest request)
        {
            var handlers = _sp.GetServices<IRequestValidator<TRequest>>()
                .OrderByDescending(x => x.Priority);

            foreach (var handler in handlers)
            {
                if(!handler.IsApplicable(context, request)) continue;

                var errors = handler.Validate(context, request)?.ToArray() ?? Array.Empty<Error>();

                if (errors.Length > 0) return Bolt.RequestBus.Response.Failed<TValue>(errors);
            }

            return null;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private async Task<Response<TValue>> ValidateAsync<TRequest, TValue>(IRequestBusContext context, TRequest request)
        {
            var handlers = _sp.GetServices<IRequestValidatorAsync<TRequest>>()
                .OrderByDescending(x => x.Priority);

            foreach (var handler in handlers)
            {
                if(!handler.IsApplicable(context, request)) continue;

                var errors = (await handler.Validate(context, request))?.ToArray() ?? Array.Empty<Error>();
                
                if(errors.Length > 0) return Bolt.RequestBus.Response.Failed<TValue>(errors);
            }

            return null;
        }
    }
}