using System;
using System.Threading.Tasks;

namespace Bolt.PubSub
{
    public interface IMessagePublisher
    {
        Task<Guid> Publish<T>(Message<T> msg);
    }
}
