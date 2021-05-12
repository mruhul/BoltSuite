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
        public ResponseCollection<TResult> Responses<TResult>()
        {
            return Responses<None, TResult>(None.Instance, isNoneRequest: true);
        }

        public ResponseCollection<TResult> Responses<TRequest, TResult>(TRequest request)
        {
            return Responses<TRequest, TResult>(request, isNoneRequest: false);
        }

        public Task<ResponseCollection<TResult>> ResponsesAsync<TResult>()
        {
            return ResponsesAsync<None, TResult>(None.Instance, isNoneRequest: true);
        }

        public Task<ResponseCollection<TResult>> ResponsesAsync<TRequest, TResult>(TRequest request)
        {
            return ResponsesAsync<TRequest, TResult>(request, isNoneRequest: false);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ResponseCollection<TResult> Responses<TRequest, TResult>(TRequest request, bool isNoneRequest)
        {
            var context = _context.Value;

            var responseUnits = new List<ResponseUnit<TResult>>();

            if (!isNoneRequest)
            {
                var valRsp = Validate<TRequest, TResult>(context, request);

                if (valRsp != null)
                {
                    responseUnits.Add(new ResponseUnit<TResult>
                    {
                        IsMainResponse = true,
                        Response = Bolt.RequestBus.Response.Failed<TResult>(valRsp.Errors)
                    });

                    return new ResponseCollection<TResult> { Responses = responseUnits };
                }
            }

            var applicableHandlers = _sp.GetServices<IResponseHandler<TRequest, TResult>>()
                .Where(x => x.IsApplicable(context, request)).ToArray();

            var mainHandler = applicableHandlers.FirstOrDefault(x => x.ExecutionHint == ExecutionHint.Main);

            if (mainHandler != null)
            {
                var rsp = mainHandler.Handle(context, request);

                if (rsp == null) rsp = Bolt.RequestBus.Response.Failed<TResult>();

                responseUnits.Add(new ResponseUnit<TResult>
                {
                    IsMainResponse = true,
                    Response = rsp
                });

                if (!rsp.IsSucceed) return new ResponseCollection<TResult> { Responses = responseUnits };
            }

            var otherHandlers = applicableHandlers
                .Where(x => x.ExecutionHint == ExecutionHint.Independent
                            || x.ExecutionHint == ExecutionHint.None);

            foreach (var otherHandler in otherHandlers)
            {
                try
                {
                    var rsp = otherHandler.Handle(context, request);

                    if (rsp != null) responseUnits.Add(new ResponseUnit<TResult>
                    {
                        IsMainResponse = false,
                        Response = rsp
                    });
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"{otherHandler.GetType().FullName} failed with error message {e.Message}");
                }
            }

            var rspCollection = new ResponseCollection<TResult>
            {
                Responses = responseUnits
            };

            var filters = _sp.GetServices<IResponseFilter<TRequest, TResult>>();

            foreach (var filter in filters)
            {
                if (!filter.IsApplicable(context, request)) continue;

                filter.Filter(context, request, rspCollection);
            }

            return rspCollection;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private async Task<ResponseCollection<TResult>> ResponsesAsync<TRequest, TResult>(TRequest request,
            bool isNoneRequest)
        {
            var context = _context.Value;

            var responseUnits = new List<ResponseUnit<TResult>>();

            if (!isNoneRequest)
            {
                var valRsp = await ValidateAsync<TRequest, TResult>(context, request);

                if (valRsp != null)
                {
                    responseUnits.Add(new ResponseUnit<TResult>
                    {
                        IsMainResponse = true,
                        Response = Bolt.RequestBus.Response.Failed<TResult>(valRsp.Errors)
                    });

                    return new ResponseCollection<TResult> { Responses = responseUnits };
                };
            }

            var applicableHandlers = _sp.GetServices<IResponseHandlerAsync<TRequest, TResult>>()
                .Where(x => x.IsApplicable(context, request)).ToArray();

            var firstBatchHandlers = applicableHandlers.Where(x
                => x.ExecutionHint == ExecutionHint.Main || x.ExecutionHint == ExecutionHint.Independent);

            var firstBatchHandlerTasks = new List<Task<Response<TResult>>>();

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
                    var rspUnit = new ResponseUnit<TResult>
                    {
                        IsMainResponse = true,
                        Response = handlerRsp ?? Bolt.RequestBus.Response.Failed<TResult>()
                    };

                    responseUnits.Add(rspUnit);

                    if (!rspUnit.Response.IsSucceed)
                    {
                        return new ResponseCollection<TResult> { Responses = responseUnits };
                    }
                }
                else
                {
                    if (handlerRsp != null)
                    {
                        responseUnits.Add(new ResponseUnit<TResult>
                        {
                            IsMainResponse = false,
                            Response = handlerRsp
                        });
                    }
                }

                index++;
            }

            var otherHandlers = applicableHandlers
                .Where(x => x.ExecutionHint == ExecutionHint.None);

            var otherHandlerTasks = new List<Task<Response<TResult>>>();

            foreach (var otherHandler in otherHandlers)
            {
                otherHandlerTasks.Add(ExecuteResponseHandler(context, otherHandler, request));
            }

            await Task.WhenAll(otherHandlerTasks);

            foreach (var task in otherHandlerTasks)
            {
                if (task.Result != null)
                {
                    responseUnits.Add(new ResponseUnit<TResult>()
                    {
                        Response = task.Result,
                        IsMainResponse = false
                    });
                }
            }

            var rspCollection = new ResponseCollection<TResult>
            {
                Responses = responseUnits
            };

            var filters = _sp.GetServices<IResponseFilterAsync<TRequest, TResult>>();

            foreach (var filter in filters)
            {
                if (!filter.IsApplicable(context, request)) continue;

                await filter.Filter(context, request, rspCollection);
            }

            return rspCollection;
        }

        private async Task<Response<TResult>> ExecuteResponseHandler<TRequest, TResult>(IRequestBusContext context,
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