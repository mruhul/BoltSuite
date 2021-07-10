namespace Bolt.PubSub
{
    public interface IMessageFilter
    {
        TMessage Filter<TMessage>(TMessage msg) where TMessage : MessageBase;
    }


}
