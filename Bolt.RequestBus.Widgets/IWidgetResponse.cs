using System.Collections.Generic;
using System.Net;

namespace Bolt.RequestBus.Widgets
{
    public interface IWidgetResponse
    {
        int StatusCode { get; }
        IRedirectAction RedirectAction { get; }
        
        string Name { get; }
        string Type { get; }
        object Data { get; }
        int DisplayOrder { get; }
    }
}