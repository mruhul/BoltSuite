using System;
using System.Threading.Tasks;

namespace Bolt.PubSub
{
    public static class MessagePublisherExtensions
    {
        public static Task<Guid> Publish<T>(this IMessagePublisher source, T msg)
        {
            return source.Publish(new Message<T>
            {
                Content = msg
            });
        }

        public static Task<Guid> Publish<T>(this IMessagePublisher source, Guid id, T msg)
        {
            return source.Publish(new Message<T>
            {
                Content = msg,
                Id = id
            });
        }
    }
}
