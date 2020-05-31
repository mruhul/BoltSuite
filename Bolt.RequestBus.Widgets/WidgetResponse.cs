using System.Collections.Generic;
using System.Net;

namespace Bolt.RequestBus.Widgets
{
    public sealed class WidgetResponse : IWidgetResponse
    {
        public int StatusCode { get; set; } = 200;
        public IRedirectAction RedirectAction { get; set; } = null;
        public string Name { get; set; }
        public string Type { get; set; }
        public object Data { get; set; }
        
        public static IWidgetResponseWithName WithName(string name) => new WidgetResponseBuilder(name);
    }

    public interface IWidgetResponseWithName : IWidgetResponseCollectType
    {
        
    }

    public interface IWidgetResponseCollectType
    {
        IWidgetResponseWithType WithType(string type);
    }

    public interface IWidgetResponseWithType : IWidgetResponseBuildInstance
    {
        
    }

    public interface IWidgetResponseBuildInstance
    {
        IWidgetResponse Ok(object data);
        IWidgetResponse StatusCode(int statusCode, object data = null);
        IResponse<IWidgetResponse> BadRequest(IEnumerable<IError> errors);
        IResponse<IWidgetResponse> Failed(params IError[] errors);
        IResponse<IWidgetResponse> Redirect(IRedirectAction redirectAction);
    }

    internal class WidgetResponseBuilder : 
        IWidgetResponseWithName,
        IWidgetResponseWithType
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

        public IWidgetResponse Ok(object data)
        {
            _rsp.StatusCode = 200;
            _rsp.Data = data;

            return _rsp;
        }

        public IWidgetResponse StatusCode(int statusCode, object data = null)
        {
            _rsp.StatusCode = statusCode;
            _rsp.Data = data;
            return _rsp;
        }

        public IResponse<IWidgetResponse> BadRequest(IEnumerable<IError> errors)
        {
            _rsp.StatusCode = 400;
            return Response.Failed<IWidgetResponse>(errors, _rsp);
        }

        public IResponse<IWidgetResponse> Failed(params IError[] errors)
        {
            _rsp.StatusCode = 500;
            return Response.Failed<IWidgetResponse>(errors, _rsp);
        }

        public IResponse<IWidgetResponse> Redirect(IRedirectAction redirectAction)
        {
            _rsp.StatusCode = redirectAction.IsPermanent 
                ? 301
                : 307;
            _rsp.RedirectAction = redirectAction;

            return Response.Succeed(_rsp);
        }
    }
}