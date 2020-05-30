using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Bolt.RequestBus.Impl
{
    internal sealed partial class RequestBus
    {
        public IResponse Send<TRequest>(TRequest request)
        {
            return Send<TRequest, None>(request, ignoreNoHandler: false);
        }

        public IResponse TrySend<TRequest>(TRequest request)
        {
            return Send<TRequest, None>(request, ignoreNoHandler: false);
        }

        public IResponse<TResult> Send<TRequest, TResult>(TRequest request)
        {
            return Send<TRequest, TResult>(request, ignoreNoHandler: false);
        }

        public IResponse<TResult> TrySend<TRequest, TResult>(TRequest request)
        {
            return Send<TRequest, TResult>(request, ignoreNoHandler: true);
        }

        public async Task<IResponse> SendAsync<TRequest>(TRequest request)
        {
            return await SendAsync<TRequest, None>(request, ignoreNoHandler: false)
                .ConfigureAwait(false);
        }

        public async Task<IResponse> TrySendAsync<TRequest>(TRequest request)
        {
            return await SendAsync<TRequest, None>(request, ignoreNoHandler: true)
                .ConfigureAwait(false);
        }

        public Task<IResponse<TResult>> SendAsync<TRequest, TResult>(TRequest request)
        {
            return SendAsync<TRequest, TResult>(request, ignoreNoHandler: false);
        }

        public Task<IResponse<TResult>> TrySendAsync<TRequest, TResult>(TRequest request)
        {
            return SendAsync<TRequest, TResult>(request, ignoreNoHandler: true);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private IResponse<TResult> Send<TRequest, TResult>(TRequest request, bool ignoreNoHandler)
        {
            var context = _context.Value;

            var valRsp = Validate<TRequest, TResult>(context, request);

            if (valRsp != null) return valRsp;

            var handlers = _sp.GetServices<IRequestHandler<TRequest, TResult>>();

            foreach (var handler in handlers)
            {
                if (!handler.IsApplicable(context, request)) continue;

                return handler.Handle(context, request);
            }

            if (ignoreNoHandler) return Bolt.RequestBus.Response.Failed<TResult>();

            throw new NoHandlerAvailable(typeof(IRequestHandler<TRequest, None>));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private async Task<IResponse<TResult>> SendAsync<TRequest, TResult>(TRequest request, bool ignoreNoHandler)
        {
            var context = _context.Value;

            var valRsp = await ValidateAsync<TRequest, TResult>(context, request);

            if (valRsp != null) return valRsp;

            var handlers = _sp.GetServices<IRequestHandlerAsync<TRequest, TResult>>();

            foreach (var handler in handlers)
            {
                if (!handler.IsApplicable(context, request)) continue;

                return await handler.Handle(context, request);
            }

            if (ignoreNoHandler) return Bolt.RequestBus.Response.Failed<TResult>();

            throw new NoHandlerAvailable(typeof(IRequestHandlerAsync<TRequest, None>));
        }
    }
}