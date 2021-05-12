using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Bolt.RequestBus
{
    public interface IRequestBus
    {
        /// <summary>
        /// Executes first applicable handler found in your ioc that implements <see cref="IRequestHandler{TRequest,None}"/>.
        /// If no applicable handler found for the request then <see cref="NoHandlerAvailable"/> exception will be thrown.
        /// Before the execution the method will also validate the request if any applicable implementations of <see cref="IRequestValidator{TRequest}"/>.
        /// When any validation failed and instance of <see cref="IResponse"/>> will be send with collection of errors (<see cref="IError"/>>).
        ///
        /// When implement a handler of <see cref="IRequestHandler{TRequest,None}"/> you can use
        /// abstract implementation of <see cref="RequestHandler{TRequest}"/>
        ///
        /// For validator you can inherit from abstract <see cref="IRequestValidator{TRequest}"/> to simplify the implementation.
        /// 
        /// <example>
        /// Sample code to use send method
        /// <code>
        ///     public class BooksController : Controller
        ///     {
        ///         private readonly IRequestBus _bus;
        /// 
        ///         public BooksController(IRequestBus bus)
        ///         {
        ///             _bus = bus;            
        ///         }
        ///
        ///         public ActionResult Get(BookByIdRequest request)
        ///         {
        ///             var rsp = _bus.Send(BookByIdRequest);
        ///             if(rsp.IsSucceed) return Ok();
        ///             return BadRequest(rsp.Errors);
        ///         }
        ///     } 
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="request"></param>
        /// <typeparam name="TRequest"></typeparam>
        /// <exception cref="NoHandlerAvailable"></exception>
        /// <returns><see cref="Response"/></returns>
        Response Send<TRequest>(TRequest request);

        /// <summary>
        /// Executes first applicable handler for the request of type TRequest. You need to register
        /// an implementations <see cref="IRequestHandler{TRequest,None}"/> and make it available in your ioc
        /// You can implement more than one handler but only first applicable one will execute
        /// The method will also validate the request if there's any applicable <see cref="IRequestValidator{TRequest}"/>
        /// defined and registered in your ioc. If no request handler found to handle the request then the method will
        /// return IResponse instance with property IsSucceed = false
        ///
        /// <para>Tips: Use abstract class <see cref="RequestHandler{TRequest}"/> to easily implement an instance
        /// of <see cref="IRequestHandler{TRequest,None}"/>
        /// </para>
        ///
        /// <para>Tips: Use abstract class <see cref="RequestValidator{TRequest}"/> to easily implement an instance
        /// of <see cref="IRequestValidator{TRequest}"/> 
        /// </para>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <typeparam name="TRequest"></typeparam>
        /// <returns><see cref="IResponse"/>></returns>
        Response TrySend<TRequest>(TRequest request);

        /// <summary>
        /// Executes first applicable handler for the request of type TRequest. You need to register
        /// an implementations <see cref="IRequestHandler{TRequest,TResult}"/> and make it available in your ioc
        /// You can implement more than one handler but only first applicable one will execute
        /// The method will also validate the request if there's any applicable <see cref="IRequestValidator{TRequest}"/>
        /// defined and registered in your ioc. If no request handler found to handle the request then the method will
        /// throw <see cref="NoHandlerAvailable"/> exception.
        /// </summary>
        /// <param name="request"></param>
        /// <typeparam name="TRequest"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <exception cref="NoHandlerAvailable"></exception>
        /// <returns>IResponse{TResult}</returns>
        Response<TResult> Send<TRequest, TResult>(TRequest request);

        /// <summary>
        /// Executes first applicable handler for the request of type TRequest. You need to register
        /// an implementations <see cref="IRequestHandler{TRequest,TResult}"/> and make it available in your ioc
        /// You can implement more than one handler but only first applicable one will execute
        /// The method will also validate the request if there's any applicable <see cref="IRequestValidator{TRequest}"/>
        /// defined and registered in your ioc. If no request handler found to handle the request then the method will
        /// return IResponse instance with property IsSucceed = false
        /// </summary>
        /// <param name="request"></param>
        /// <typeparam name="TRequest"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <returns>IResponse{TResult}</returns>
        Response<TResult> TrySend<TRequest, TResult>(TRequest request);

        /// <summary>
        /// Executes all applicable handlers for the event. You need to register implementations of <see cref="IEventHandler{TEvent}"/>
        /// and make them available in your ioc.
        /// </summary>
        /// <param name="event"></param>
        /// <typeparam name="TEvent"></typeparam>
        void Publish<TEvent>(TEvent @event);

        /// <summary>
        /// Executes all applicable handlers for the event in parallel. You need to register
        /// implementations of <see cref="IEventHandler{TEvent}"/> and make them available in your ioc.
        /// </summary>
        /// <param name="event"></param>
        /// <typeparam name="TEvent"></typeparam>
        /// <returns></returns>
        Task PublishAsync<TEvent>(TEvent @event);

        /// <summary>
        /// Executes first applicable handler found in your ioc that implements <see cref="IRequestHandlerAsync{TRequest,None}"/>.
        /// If no applicable handler found for the request then <see cref="NoHandlerAvailable"/> exception will be thrown.
        /// Before the execution the method will also validate the request if any applicable implementations of <see cref="IRequestValidatorAsync{TRequest}"/>.
        /// When any validation failed and instance of <see cref="Response"/>> will be send with collection of errors (<see cref="Error"/>).
        /// </summary>
        /// <param name="request"></param>
        /// <typeparam name="TRequest"></typeparam>
        /// <returns>IResponse</returns>
        Task<Response> SendAsync<TRequest>(TRequest request);

        /// <summary>
        /// Executes first applicable handler for the request of type TRequest. You need to register
        /// an implementations <see cref="IRequestHandlerAsync{TRequest,None}"/> and make it available in your ioc
        /// You can implement more than one handler but only first applicable one will execute
        /// The method will also validate the request if there's any applicable <see cref="IRequestValidatorAsync{TRequest}"/>
        /// defined and registered in your ioc. If no request handler found to handle the request then the method will
        /// return IResponse instance with property IsSucceed = false
        /// </summary>
        /// <param name="request"></param>
        /// <typeparam name="TRequest"></typeparam>
        /// <returns>IResponse</returns>
        Task<Response> TrySendAsync<TRequest>(TRequest request);

        /// <summary>
        /// Executes first applicable handler found in your ioc that implements <see cref="IRequestHandlerAsync{TRequest,TResult}"/>.
        /// If no applicable handler found for the request then <see cref="NoHandlerAvailable"/> exception will be thrown.
        /// Before the execution the method will also validate the request if any applicable implementations of <see cref="IRequestValidatorAsync{TRequest}"/>.
        /// When any validation failed and instance of <see cref="Response"/>> will be send with collection of errors (<see cref="Error"/>).
        /// </summary>
        /// <param name="request"></param>
        /// <typeparam name="TRequest"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <returns>IResponse{TResult}</returns>
        Task<Response<TResult>> SendAsync<TRequest, TResult>(TRequest request);

        /// <summary>
        /// Executes first applicable handler for the request of type TRequest. You need to register
        /// an implementations <see cref="IRequestHandlerAsync{TRequest,TResult}"/> and make it available in your ioc
        /// You can implement more than one handler but only first applicable one will execute
        /// The method will also validate the request if there's any applicable <see cref="IRequestValidatorAsync{TRequest}"/>
        /// defined and registered in your ioc. If no request handler found to handle the request then the method will
        /// return IResponse instance with property IsSucceed = false
        /// </summary>
        /// <param name="request"></param>
        /// <typeparam name="TRequest"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <returns>IResponse{TResult}</returns>
        Task<Response<TResult>> TrySendAsync<TRequest, TResult>(TRequest request);

        /// <summary>
        /// Executes first applicable handler that implements <see cref="IResponseHandler{None,TResult}"/>
        /// and return the result as <see cref="Response{TResult}"/>. When no handler found the method will throw
        /// <see cref="NoHandlerAvailable"/> exception.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <exception cref="NoHandlerAvailable"></exception>
        /// <returns>IResponse{TResult}</returns>
        Response<TResult> Response<TResult>();
        
        /// <summary>
        /// Executes first applicable handler that implements <see cref="IResponseHandler{None,TResult}"/>
        /// and return the result as <see cref="Response{TResult}"/>. When no handler found the method will return
        /// an instance of <see cref="Response{TResult}"/> whose IsSucceed value will be false.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <returns>IResponse{TResult}</returns>
        Response<TResult> TryResponse<TResult>();

        /// <summary>
        /// Executes all applicable handlers that implement <see cref="IResponseHandler{None,TResult}"/>
        /// and return all the responses as a collection of <see cref="Response{TResult}"/>. If main handler failed
        /// then the exception propagates to the caller. Otherwise the method continue executing other handlers and
        /// return responses fo the successful handlers. The collection response can be add/remove/modify using
        /// <see cref="IResponseFilter{None,TResult}"/>. When all the handler execution finished the collected result pass
        /// to the available ResponseFilter to allow them to change response. Handy if you want to execute something
        /// when execution of all handlers finished.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <returns>IEnumerable{IResponse{TResult}}</returns>
        ResponseCollection<TResult> Responses<TResult>();
        
        /// <summary>
        /// Validate the input of TRequest using any applicable <see cref="IRequestValidator{TRequest}"/> and when
        /// validation succeed then executes all applicable handlers that implement <see cref="IResponseHandler{TRequest,TResult}"/>
        /// and return all the responses as a collection of <see cref="Response{TResult}"/>. If main handler failed
        /// then the exception propagates to the caller. Otherwise the method continue executing other handlers and
        /// return responses fo the successful handlers. The collection response can be add/remove/modify using
        /// <see cref="IResponseFilter{TRequest,TResult}"/>. When all the handler execution finished the collected result pass
        /// to the available ResponseFilter to allow them to change response. Handy if you want to execute something
        /// when execution of all handlers finished.
        /// </summary>
        /// <param name="request"></param>
        /// <typeparam name="TRequest"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <returns>IEnumerable{IResponse{TResult}}</returns>
        ResponseCollection<TResult> Responses<TRequest, TResult>(TRequest request);

        /// <summary>
        /// Executes first applicable handler that implements <see cref="IResponseHandlerAsync{None,TResult}"/>
        /// and return the result as <see cref="IResponse{TResult}"/>. When no handler found the method will throw
        /// <see cref="NoHandlerAvailable"/> exception.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <returns>IResponse{TResult}</returns>
        Task<Response<TResult>> ResponseAsync<TResult>();
        
        /// <summary>
        /// Executes first applicable handler that implements <see cref="IResponseHandlerAsync{None,TResult}"/>
        /// and return the result as <see cref="Response{TResult}"/>. When no handler found the method will return
        /// an instance of <see cref="Response{TResult}"/> whose IsSucceed value will be false.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <returns>IResponse{TResult}</returns>
        Task<Response<TResult>> TryResponseAsync<TResult>();
        
        /// <summary>
        /// Executes all applicable handlers that implement <see cref="IResponseHandlerAsync{None,TResult}"/>
        /// and return all the responses as a collection of <see cref="Response{TResult}"/>. If main handler failed
        /// then the exception propagates to the caller. Otherwise the method continue executing other handlers and
        /// return responses fo the successful handlers. The collection response can be add/remove/modify using
        /// <see cref="IResponseFilterAsync{None,TResult}"/>. When all the handler execution finished the collected result pass
        /// to the available ResponseFilter to allow them to change response. Handy if you want to execute something
        /// when execution of all handlers finished.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <returns>Task{IEnumerable{IResponse{TResult}}}</returns>
        Task<ResponseCollection<TResult>> ResponsesAsync<TResult>();
        
        /// <summary>
        /// Validate the input of TRequest using any applicable <see cref="IRequestValidatorAsync{TRequest}"/> and when
        /// validation succeed then executes all applicable handlers that implement <see cref="IResponseHandler{TRequest,TResult}"/>
        /// and return all the responses as a collection of <see cref="Response{TResult}"/>. If main handler failed
        /// then the exception propagates to the caller. Otherwise the method continue executing other handlers and
        /// return responses fo the successful handlers. The collection response can be add/remove/modify using
        /// <see cref="IResponseFilter{TRequest,TResult}"/>. When all the handler execution finished the collected result pass
        /// to the available ResponseFilter to allow them to change response. Handy if you want to execute something
        /// when execution of all handlers finished.
        /// </summary>
        /// <param name="request"></param>
        /// <typeparam name="TRequest"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <returns>Task{IEnumerable{IResponse{TResult}}}</returns>
        Task<ResponseCollection<TResult>> ResponsesAsync<TRequest, TResult>(TRequest request);
    }
}