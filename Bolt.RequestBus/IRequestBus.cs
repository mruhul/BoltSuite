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
        /// <returns><see cref="IResponse"/></returns>
        IResponse Send<TRequest>(TRequest request);

        /// <summary>
        /// Executes first applicable handler for the request of type TRequest. You need to register
        /// an implementations <see cref="IRequestHandler{TRequest,None}"/> and make it available in your ioc
        /// You can implement more than one handler but only first applicable one will execute
        /// The method will also validate the request if there's any applicable <see cref="IRequestValidator{TRequest}"/>
        /// defined and registered in your ioc. If no request handler found to handle the request then the method will
        /// return IResponse instance with property IsSucceed = false
        /// </summary>
        /// <param name="request"></param>
        /// <typeparam name="TRequest"></typeparam>
        /// <returns><see cref="IResponse"/>></returns>
        IResponse TrySend<TRequest>(TRequest request);

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
        IResponse<TResult> Send<TRequest, TResult>(TRequest request);

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
        IResponse<TResult> TrySend<TRequest, TResult>(TRequest request);

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
        /// When any validation failed and instance of <see cref="IResponse"/>> will be send with collection of errors (<see cref="IError"/>).
        /// </summary>
        /// <param name="request"></param>
        /// <typeparam name="TRequest"></typeparam>
        /// <returns>IResponse</returns>
        Task<IResponse> SendAsync<TRequest>(TRequest request);

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
        Task<IResponse> TrySendAsync<TRequest>(TRequest request);

        /// <summary>
        /// Executes first applicable handler found in your ioc that implements <see cref="IRequestHandlerAsync{TRequest,TResult}"/>.
        /// If no applicable handler found for the request then <see cref="NoHandlerAvailable"/> exception will be thrown.
        /// Before the execution the method will also validate the request if any applicable implementations of <see cref="IRequestValidatorAsync{TRequest}"/>.
        /// When any validation failed and instance of <see cref="IResponse"/>> will be send with collection of errors (<see cref="IError"/>).
        /// </summary>
        /// <param name="request"></param>
        /// <typeparam name="TRequest"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <returns>IResponse{TResult}</returns>
        Task<IResponse<TResult>> SendAsync<TRequest, TResult>(TRequest request);

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
        Task<IResponse<TResult>> TrySendAsync<TRequest, TResult>(TRequest request);

        /// <summary>
        /// Executes first applicable handler that implements <see cref="IResponseHandler{None,TResult}"/>
        /// and return the result as <see cref="IResponse{TResult}"/>. When no handler found the method will throw
        /// <see cref="NoHandlerAvailable"/> exception.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <exception cref="NoHandlerAvailable"></exception>
        /// <returns>IResponse{TResult}</returns>
        IResponse<TResult> Response<TResult>();
        
        /// <summary>
        /// Executes first applicable handler that implements <see cref="IResponseHandler{None,TResult}"/>
        /// and return the result as <see cref="IResponse{TResult}"/>. When no handler found the method will return
        /// an instance of <see cref="IResponse{TResult}"/> whose IsSucceed value will be false.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <returns>IResponse{TResult}</returns>
        IResponse<TResult> TryResponse<TResult>();

        IEnumerable<IResponse<TResult>> Responses<TResult>();
        IEnumerable<IResponse<TResult>> Responses<TRequest, TResult>(TRequest request);

        Task<IResponse<TResult>> ResponseAsync<TResult>();
        Task<IResponse<TResult>> TryResponseAsync<TResult>();
        Task<IEnumerable<IResponse<TResult>>> ResponsesAsync<TResult>();
        Task<IEnumerable<IResponse<TResult>>> ResponsesAsync<TRequest, TResult>(TRequest request);
    }
}