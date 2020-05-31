using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Bolt.RequestBus.Widgets
{
    public interface IWidgetGroupResponse
    {
        int StatusCode { get; }
        IEnumerable<IError> Errors { get; }
        IRedirectAction RedirectAction { get; }
        IEnumerable<IWidgetUnitResponse> Widgets { get; }
    }
    
    internal sealed class WidgetGroupResponse : IWidgetGroupResponse
    {
        private readonly List<IWidgetUnitResponse> _response = new List<IWidgetUnitResponse>(); 
        public int StatusCode { get; set; }
        public IEnumerable<IError> Errors { get; set; } = Enumerable.Empty<IError>();
        
        public IRedirectAction RedirectAction { get; set; }
        public IEnumerable<IWidgetUnitResponse> Widgets => _response;

        internal void AddResponse(IWidgetUnitResponse rsp) => _response.Add(rsp);
    }
}