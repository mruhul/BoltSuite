using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Bolt.RequestBus.Widgets
{
    public sealed class WidgetResponse
    {
        public int StatusCode { get; set; } = 200;
        public RedirectAction RedirectAction { get; set; } = null;
        public string Name { get; set; }
        public string Type { get; set; }
        public object Data { get; set; }
        
        /// <summary>
        /// Smaller number get should display first
        /// </summary>
        public int DisplayOrder { get; set; }
        
        public static IWidgetResponseWithName WithName(string name) => new WidgetResponseBuilder(name);
    }

    public interface IWidgetResponseWithName : IWidgetResponseCollectType
    {
        
    }

    public interface IWidgetResponseCollectDisplayOrder
    {
        IWidgetResponseWithDisplayOrder WithDisplayOrder(int order);
    }

    public interface IWidgetResponseWithDisplayOrder : IWidgetResponseBuildInstance
    {
        
    }

    public interface IWidgetResponseCollectType
    {
        IWidgetResponseWithType WithType(string type);
    }

    public interface IWidgetResponseWithType : IWidgetResponseBuildInstance, IWidgetResponseCollectDisplayOrder
    {
        
    }

    public interface IWidgetResponseBuildInstance
    {
        WidgetResponse Ok(object data);
        WidgetResponse StatusCode(int statusCode, object data = null);
        Response<WidgetResponse> BadRequest(IEnumerable<Error> errors);
        Response<WidgetResponse> Failed(params Error[] errors);
        Response<WidgetResponse> Redirect(RedirectAction redirectAction);
    }

    internal class WidgetResponseBuilder : 
        IWidgetResponseWithName,
        IWidgetResponseWithType,
        IWidgetResponseWithDisplayOrder
    {
        private readonly WidgetResponse _rsp = new WidgetResponse();

        public WidgetResponseBuilder(string name)
        {
            _rsp.Name = name;
        }

        public IWidgetResponseWithType WithType(string type)
        {
            _rsp.Type = type;
            return this;
        }

        public WidgetResponse Ok(object data)
        {
            _rsp.StatusCode = 200;
            _rsp.Data = data;

            return _rsp;
        }

        public WidgetResponse StatusCode(int statusCode, object data = null)
        {
            _rsp.StatusCode = statusCode;
            _rsp.Data = data;
            return _rsp;
        }

        public Response<WidgetResponse> BadRequest(IEnumerable<Error> errors)
        {
            _rsp.StatusCode = 400;
            return Response.Failed<WidgetResponse>(errors.ToArray(), _rsp);
        }

        public Response<WidgetResponse> Failed(params Error[] errors)
        {
            _rsp.StatusCode = 500;
            return Response.Failed<WidgetResponse>(errors, _rsp);
        }

        public Response<WidgetResponse> Redirect(RedirectAction redirectAction)
        {
            _rsp.StatusCode = redirectAction.IsPermanent 
                ? 301
                : 307;
            _rsp.RedirectAction = redirectAction;

            return Response.Ok(_rsp.StatusCode, _rsp as WidgetResponse);
        }

        public IWidgetResponseWithDisplayOrder WithDisplayOrder(int order)
        {
            _rsp.DisplayOrder = order;
            return this;
        }
    }
}