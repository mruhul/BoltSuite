namespace Bolt.RequestBus
{
    public interface IEventHandler<in TEvent>
    {
        void Handle(IRequestBusContext context, TEvent @event);
        bool IsApplicable(IRequestBusContext context, TEvent @event);
    }
    
    public abstract class EventHandler<TEvent> : IEventHandler<TEvent>
    {
        public abstract void Handle(IRequestBusContext context, TEvent @event);

        public virtual bool IsApplicable(IRequestBusContext context, TEvent @event) => true;
    }
}