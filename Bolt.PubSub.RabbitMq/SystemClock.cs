using System;

namespace Bolt.PubSub.RabbitMq
{
    internal interface ISystemClock
    {
        DateTime UtcNow { get; }
    }

    internal sealed class SystemClock : ISystemClock
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }
}
