using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Bolt.RequestBus.Widgets
{
    public sealed class WidgetHandlerResponse : IWidgetHandlerResponse
    {
        public int StatusCode { get; set; } = 200;
        public IRedirectAction RedirectAction { get; set; } = null;
        public IEnumerable<IWidgetUnit> Widgets { get; set; } = Enumerable.Empty<IWidgetUnit>();
    }

    public sealed class WidgetUnit : IWidgetUnit
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public object Data { get; set; }

        /// <summary>
        /// Smaller number get should display first
        /// </summary>
        public int DisplayOrder { get; set; }
    }
}