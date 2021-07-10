using System;

namespace Bolt.PubSub
{
    public interface IUniqueId
    {
        Guid New();
    }
}
