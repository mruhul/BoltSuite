using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Bolt.RequestBus
{
    public interface IRequestBus
    {
        IResponse Send<TRequest>(TRequest request);
        IResponse TrySend<TRequest>(TRequest request);
        
        IResponse<TResult> Send<TRequest,TResult>(TRequest request);
        IResponse<TResult> TrySend<TRequest,TResult>(TRequest request);
        
        void Publish<TEvent>(TEvent @event);
        
        Task<IResponse> SendAsync<TRequest>(TRequest request);
        Task<IResponse> TrySendAsync<TRequest>(TRequest request);
        
        Task<IResponse<TResult>> SendAsync<TRequest,TResult>(TRequest request);
        Task<IResponse<TResult>> TrySendAsync<TRequest,TResult>(TRequest request);
        
        Task PublishAsync<TEvent>(TEvent @event);

        IResponse<TResult> Response<TResult>();
        IEnumerable<IResponse<TResult>> Responses<TResult>();
        IEnumerable<IResponse<TResult>> Responses<TRequest,TResult>(TRequest request);
        
        Task<IResponse<TResult>> ResponseAsync<TResult>();
        Task<IEnumerable<IResponse<TResult>>> ResponsesAsync<TResult>();
        Task<IEnumerable<IResponse<TResult>>> ResponsesAsync<TRequest,TResult>(TRequest request);
    }

    public sealed class RequestBus : IRequestBus
    {
        private readonly IServiceProvider _sp;
        private readonly Lazy<IRequestBusContext> _context;

        public RequestBus(IServiceProvider sp)
        {
            _sp = sp;
            _context = new Lazy<IRequestBusContext>(() => BuildContext(_sp));
        }

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

        public void Publish<TEvent>(TEvent @event)
        {
            var context = _context.Value;
            
            var handlers = _sp.GetServices<IEventHandler<TEvent>>();

            foreach (var handler in handlers)
            {
                if(!handler.IsApplicable(context, @event)) continue;
                
                handler.Handle(context, @event);
            }
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

        public async Task PublishAsync<TEvent>(TEvent @event)
        {
            var context = _context.Value;
            
            var handlers = _sp.GetServices<IEventHandlerAsync<TEvent>>();
            
            var tasks = new List<Task>();
            
            foreach (var handler in handlers)
            {
                if(!handler.IsApplicable(context, @event)) continue;
                
                tasks.Add(handler.Handle(context, @event));
            }

            await Task.WhenAll(tasks);
        }

        public IResponse<TResult> Response<TResult>()
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<IResponse<TResult>> Responses<TResult>()
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<IResponse<TResult>> Responses<TRequest, TResult>(TRequest request)
        {
            throw new System.NotImplementedException();
        }

        public Task<IResponse<TResult>> ResponseAsync<TResult>()
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<IResponse<TResult>>> ResponsesAsync<TResult>()
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<IResponse<TResult>>> ResponsesAsync<TRequest, TResult>(TRequest request)
        {
            throw new System.NotImplementedException();
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

        private IResponse<TResult> Send<TRequest, TResult>(TRequest request, bool ignoreNoHandler)
        {
            var context = _context.Value;
            
            var handlers = _sp.GetServices<IRequestHandler<TRequest, TResult>>();
            
            foreach (var handler in handlers)
            {
                if(!handler.IsApplicable(context, request)) continue;

                return handler.Handle(context, request);
            }

            if (ignoreNoHandler) return Bolt.RequestBus.Response.Failed<TResult>();
            
            throw new NoRequestHandlerAvailable(typeof(IRequestHandler<TRequest,None>));
        }
        
        private async Task<IResponse<TResult>> SendAsync<TRequest, TResult>(TRequest request, bool ignoreNoHandler)
        {
            var context = _context.Value;
            
            var handlers = _sp.GetServices<IRequestHandlerAsync<TRequest, TResult>>();
            
            foreach (var handler in handlers)
            {
                if(!handler.IsApplicable(context, request)) continue;

                return await handler.Handle(context, request);
            }

            if (ignoreNoHandler) return Bolt.RequestBus.Response.Failed<TResult>();
            
            throw new NoRequestHandlerAvailable(typeof(IRequestHandlerAsync<TRequest,None>));
        }
    }
}