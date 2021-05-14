using System.Collections.Generic;
using System.Linq;

namespace Bolt.RequestBus.Widgets
{
    public sealed class WidgetResponse
    {
        public RedirectAction RedirectAction { get; init; }
        public IEnumerable<SingleWidgetResponseDto> Widgets { get; init; } = Enumerable.Empty<SingleWidgetResponseDto>();
    }

    public sealed record SingleWidgetResponseDto
    {
        public string Name { get; init; }
        public string Type { get; init; }
        public object Data { get; init; }

        /// <summary>
        /// Smaller number get should display first
        /// </summary>
        public int DisplayOrder { get; init; }
    }
}