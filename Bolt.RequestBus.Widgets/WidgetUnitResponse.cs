﻿using System.Collections.Generic;

namespace Bolt.RequestBus.Widgets
{
    public interface IWidgetUnitResponse
    {
        int StatusCode { get; }
        IEnumerable<Error> Errors { get; }
        string Name { get; }
        string Type { get; }
        object Data { get; }
        int DisplayOrder { get; }
    }

    internal class WidgetUnitResponse : IWidgetUnitResponse
    {
        public int StatusCode { get; set; }
        public IEnumerable<Error> Errors { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public object Data { get; set; }
        public int DisplayOrder { get; set; }
    }
}