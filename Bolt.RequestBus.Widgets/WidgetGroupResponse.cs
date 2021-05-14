using System.Collections.Generic;
using System.Linq;

namespace Bolt.RequestBus.Widgets
{
    public sealed record WidgetGroupResponse
    {
        public int StatusCode { get; init; }
        public string StatusReason { get; init; }
        public IEnumerable<Error> Errors { get; init; } = Enumerable.Empty<Error>();
        public RedirectAction RedirectAction { get; init; }
        public IEnumerable<WidgetUnitResponse> Widgets { get; init; }
    }

    public sealed record WidgetUnitResponse
    {
        public int? StatusCode { get; init; }
        public IEnumerable<Error> Errors { get; init; }
        public string Name { get; init; }
        public string Type { get; init; }
        public object Data { get; init; }
        public int DisplayOrder { get; init; }
    }
}