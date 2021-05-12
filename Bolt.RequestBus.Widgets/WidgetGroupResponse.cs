using System.Collections.Generic;
using System.Linq;

namespace Bolt.RequestBus.Widgets
{
    public sealed class WidgetGroupResponse
    {
        private readonly List<WidgetUnitResponse> _response = new List<WidgetUnitResponse>(); 
        public int StatusCode { get; set; }
        public IEnumerable<Error> Errors { get; set; } = Enumerable.Empty<Error>();
        
        public RedirectAction RedirectAction { get; set; }
        public IEnumerable<WidgetUnitResponse> Widgets => _response;

        internal void AddResponse(WidgetUnitResponse rsp) => _response.Add(rsp);
    }
}