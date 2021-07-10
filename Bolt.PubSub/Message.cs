using System;
using System.Collections.Generic;

namespace Bolt.PubSub
{
    public record MessageBase
    {
        public string AppId { get; init; }
        public string CorrelationId { get; init; }
        public int Version { get; init; }
        public string Type { get; init; }
        public DateTime? CreatedAt { get; init; }
        public Dictionary<string, string> Headers { get; init; } = new Dictionary<string, string>();
    }

    public record Message<T> : MessageBase
    {
        public Guid? Id { get; init; }
        public T Content { get; init; }
    }
}
