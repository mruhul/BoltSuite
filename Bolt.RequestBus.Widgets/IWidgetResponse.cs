using System.Collections.Generic;
using System.Net;

namespace Bolt.RequestBus.Widgets
{
    public interface IWidgetResponse
    {
        IRedirectAction RedirectAction { get; }
        
        string Name { get; }
        string Type { get; }
        object Data { get; }
        int DisplayOrder { get; }
    }
}