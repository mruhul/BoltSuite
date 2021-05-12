using System.Collections.Generic;

namespace Bolt.RequestBus.Widgets
{
    public class WidgetUnitResponse
    {
        public int StatusCode { get; set; }
        public IEnumerable<Error> Errors { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public object Data { get; set; }
        public int DisplayOrder { get; set; }
    }
}