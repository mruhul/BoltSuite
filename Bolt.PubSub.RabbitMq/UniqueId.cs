using System;

namespace Bolt.PubSub.RabbitMq
{
    internal sealed class UniqueId : IUniqueId
    {
        public Guid New() => Guid.NewGuid();
    }
}
