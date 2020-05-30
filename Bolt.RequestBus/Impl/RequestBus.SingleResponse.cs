using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Bolt.RequestBus.Impl
{
    internal sealed partial class RequestBus
    {
        public IResponse<TResult> Response<TResult>()
        {
            return Response<None, TResult>(None.Instance, ignoreNoHandler: false);
        }

        public IResponse<TResult> TryResponse<TResult>()
        {
            return Response<None, TResult>(None.Instance, ignoreNoHandler: true);
        }

        public Task<IResponse<TResult>> ResponseAsync<TResult>()
        {
            return ResponseAsync<None, TResult>(None.Instance, ignoreNoHandler: false);
        }

        public Task<IResponse<TResult>> TryResponseAsync<TResult>()
        {
            return ResponseAsync<None, TResult>(None.Instance, ignoreNoHandler: true);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private IResponse<TResult> Response<TRequest, TResult>(TRequest request, bool ignoreNoHandler)
        {
            var handlers = _sp.GetServices<IResponseHandler<None, TResult>>();

            var context = _context.Value;

            foreach (var handler in handlers)
            {
                if (!handler.IsApplicable(context, None.Instance)) continue;

                return handler.Handle(context, None.Instance);
            }

            if (ignoreNoHandler) return Bolt.RequestBus.Response.Failed<TResult>();

            throw new NoHandlerAvailable(typeof(IResponseHandler<None, TResult>));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private async Task<IResponse<TResult>> ResponseAsync<TRequest, TResult>(TRequest request, bool ignoreNoHandler)
        {
            var handlers = _sp.GetServices<IResponseHandlerAsync<None, TResult>>();

            var context = _context.Value;

            foreach (var handler in handlers)
            {
                if (!handler.IsApplicable(context, None.Instance)) continue;

                return await handler.Handle(context, None.Instance);
            }

            if (ignoreNoHandler) return Bolt.RequestBus.Response.Failed<TResult>();

            throw new NoHandlerAvailable(typeof(IResponseHandler<None, TResult>));
        }
    }
}