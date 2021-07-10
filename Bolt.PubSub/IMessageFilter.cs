namespace Bolt.PubSub
{
    public interface IMessageFilter
    {
        Message<T> Filter<T>(Message<T> msg);
    }
}
