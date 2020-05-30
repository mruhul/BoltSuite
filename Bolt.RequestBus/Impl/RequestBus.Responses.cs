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
        public IEnumerable<IResponse<TResult>> Responses<TResult>()
        {
            return Responses<None, TResult>(None.Instance, isNoneRequest: true);
        }

        public IEnumerable<IResponse<TResult>> Responses<TRequest, TResult>(TRequest request)
        {
            return Responses<TRequest, TResult>(request, isNoneRequest: false);
        }

        public Task<IEnumerable<IResponse<TResult>>> ResponsesAsync<TResult>()
        {
            return ResponsesAsync<None, TResult>(None.Instance, isNoneRequest: true);
        }

        public Task<IEnumerable<IResponse<TResult>>> ResponsesAsync<TRequest, TResult>(TRequest request)
        {
            return ResponsesAsync<TRequest, TResult>(request, isNoneRequest: false);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private IEnumerable<IResponse<TResult>> Responses<TRequest, TResult>(TRequest request, bool isNoneRequest)
        {
            var context = _context.Value;

            if (!isNoneRequest)
            {
                var valRsp = Validate<TRequest, TResult>(context, request);

                if (valRsp != null) return new[] {valRsp};
            }

            var applicableHandlers = _sp.GetServices<IResponseHandler<TRequest, TResult>>()
                .Where(x => x.IsApplicable(context, request)).ToArray();

            var result = new List<IResponse<TResult>>();

            var mainHandler = applicableHandlers.FirstOrDefault(x => x.ExecutionHint == ExecutionHint.Main);

            if (mainHandler != null)
            {
                var rsp = mainHandler.Handle(context, request);

                result.Add(rsp);
                
                if (!rsp.IsSucceed) return result;
            }

            var otherHandlers = applicableHandlers
                .Where(x => x.ExecutionHint == ExecutionHint.Independent
                            || x.ExecutionHint == ExecutionHint.None);

            foreach (var otherHandler in otherHandlers)
            {
                try
                {
                    result.Add(otherHandler.Handle(context, request));
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
                
                filter.Filter(context, request, result);
            }
            
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private async Task<IEnumerable<IResponse<TResult>>> ResponsesAsync<TRequest, TResult>(TRequest request,
            bool isNoneRequest)
        {
            var context = _context.Value;

            if (!isNoneRequest)
            {
                var valRsp = await ValidateAsync<TRequest, TResult>(context, request);

                if (valRsp != null) return new[] {valRsp};
            }

            var applicableHandlers = _sp.GetServices<IResponseHandlerAsync<TRequest, TResult>>()
                .Where(x => x.IsApplicable(context, request)).ToArray();

            var result = new List<IResponse<TResult>>();

            var firstBatchHandlers = applicableHandlers.Where(x
                => x.ExecutionHint == ExecutionHint.Main || x.ExecutionHint == ExecutionHint.Independent);

            var firstBatchHandlerTasks = new List<Task<IResponse<TResult>>>();
            
            var mainHandlerIndex = -1;
            var index = 0;
            foreach (var handler in firstBatchHandlers)
            {
                if (handler.ExecutionHint == ExecutionHint.Main) mainHandlerIndex = index;

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
                    // main handler response can't be null as it throws exception if anything goes wrong
                    if (!handlerRsp.IsSucceed)
                    {
                        return new[] {handlerRsp};
                    }
                }
                
                if(handlerRsp != null) result.Add(batchHandler.Result);
                
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
                if(task.Result != null) result.Add(task.Result);
            }
            
            var filters = _sp.GetServices<IResponseFilterAsync<TRequest, TResult>>();

            foreach (var filter in filters)
            {
                if(!filter.IsApplicable(context, request)) continue;
                
                await filter.Filter(context, request, result);
            }

            return result;
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