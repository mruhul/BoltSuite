using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Bolt.RequestBus.Impl
{
    internal sealed partial class RequestBus
    {
        public IResponseCollection<TResult> Responses<TResult>()
        {
            return Responses<None, TResult>(None.Instance, isNoneRequest: true);
        }

        public IResponseCollection<TResult> Responses<TRequest, TResult>(TRequest request)
        {
            return Responses<TRequest, TResult>(request, isNoneRequest: false);
        }

        public Task<IResponseCollection<TResult>> ResponsesAsync<TResult>()
        {
            return ResponsesAsync<None, TResult>(None.Instance, isNoneRequest: true);
        }

        public Task<IResponseCollection<TResult>> ResponsesAsync<TRequest, TResult>(TRequest request)
        {
            return ResponsesAsync<TRequest, TResult>(request, isNoneRequest: false);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private IResponseCollection<TResult> Responses<TRequest, TResult>(TRequest request, bool isNoneRequest)
        {
            var context = _context.Value;
            var rspCollection = new ResponseCollection<TResult>();
            
            if (!isNoneRequest)
            {
                var valRsp = Validate<TRequest, TResult>(context, request);

                if (valRsp != null)
                {
                    rspCollection.MainResponse = valRsp;
                    
                    return rspCollection;
                }
            }

            var applicableHandlers = _sp.GetServices<IResponseHandler<TRequest, TResult>>()
                .Where(x => x.IsApplicable(context, request)).ToArray();

            var mainHandler = applicableHandlers.FirstOrDefault(x => x.ExecutionHint == ExecutionHint.Main);

            if (mainHandler != null)
            {
                var rsp = mainHandler.Handle(context, request);
                
                rspCollection.MainResponse = rsp;
                
                if (!rsp.IsSucceed) return rspCollection;
            }

            var otherHandlers = applicableHandlers
                .Where(x => x.ExecutionHint == ExecutionHint.Independent
                            || x.ExecutionHint == ExecutionHint.None);

            foreach (var otherHandler in otherHandlers)
            {
                try
                {
                    rspCollection.AddResponse(otherHandler.Handle(context, request));
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"{otherHandler.GetType().FullName} failed with error message {e.Message}");
                }
            }

            var filters = _sp.GetServices<IResponseFilter<TRequest, TResult>>();

            foreach (var filter in filters)
            {
                if(!filter.IsApplicable(context, request)) continue;
                
                filter.Filter(context, request, rspCollection);
            }
            
            return rspCollection;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private async Task<IResponseCollection<TResult>> ResponsesAsync<TRequest, TResult>(TRequest request,
            bool isNoneRequest)
        {
            var context = _context.Value;
            
            var rspCollection = new ResponseCollection<TResult>();

            if (!isNoneRequest)
            {
                var valRsp = await ValidateAsync<TRequest, TResult>(context, request);

                if (valRsp != null)
                {
                    rspCollection.MainResponse = valRsp;
                    
                    return rspCollection;
                };
            }

            var applicableHandlers = _sp.GetServices<IResponseHandlerAsync<TRequest, TResult>>()
                .Where(x => x.IsApplicable(context, request)).ToArray();

            var firstBatchHandlers = applicableHandlers.Where(x
                => x.ExecutionHint == ExecutionHint.Main || x.ExecutionHint == ExecutionHint.Independent);

            var firstBatchHandlerTasks = new List<Task<IResponse<TResult>>>();
            
            var mainHandlerIndex = -1;
            var index = 0;
            foreach (var handler in firstBatchHandlers)
            {
                if (mainHandlerIndex == -1 && handler.ExecutionHint == ExecutionHint.Main) mainHandlerIndex = index;

                firstBatchHandlerTasks.Add(ExecuteResponseHandler(context, handler, request));

                index++;
            }

            await Task.WhenAll(firstBatchHandlerTasks);

            index = 0;
            foreach (var batchHandler in firstBatchHandlerTasks)
            {
                var handlerRsp = batchHandler.Result;
                
                if (index == mainHandlerIndex)
                {
                    rspCollection.MainResponse = handlerRsp;
                    
                    if (!handlerRsp.IsSucceed)
                    {
                        return rspCollection;
                    }
                }
                else
                {
                    rspCollection.AddResponse(batchHandler.Result);
                }
                
                index++;
            }

            var otherHandlers = applicableHandlers
                .Where(x => x.ExecutionHint == ExecutionHint.None);

            var otherHandlerTasks = new List<Task<IResponse<TResult>>>();

            foreach (var otherHandler in otherHandlers)
            {
                otherHandlerTasks.Add(ExecuteResponseHandler(context, otherHandler, request));
            }

            await Task.WhenAll(otherHandlerTasks);

            foreach (var task in otherHandlerTasks)
            {
                rspCollection.AddResponse(task.Result);
            }
            
            var filters = _sp.GetServices<IResponseFilterAsync<TRequest, TResult>>();

            foreach (var filter in filters)
            {
                if(!filter.IsApplicable(context, request)) continue;
                
                await filter.Filter(context, request, rspCollection);
            }

            return rspCollection;
        }

        private async Task<IResponse<TResult>> ExecuteResponseHandler<TRequest, TResult>(IRequestBusContext context,
            IResponseHandlerAsync<TRequest, TResult> handler,
            TRequest request)
        {
            if (handler.ExecutionHint == ExecutionHint.Main) return await handler.Handle(context, request);

            try
            {
                return await handler.Handle(context, request);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Handler {handler.GetType().FullName} failed with message {e.Message}");
            }

            return null;
        }
    }
}