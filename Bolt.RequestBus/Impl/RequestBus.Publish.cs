using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Bolt.RequestBus.Impl
{
    internal sealed partial class RequestBus
    {
        public void Publish<TEvent>(TEvent @event)
        {
            var context = _context.Value;

            var handlers = _sp.GetServices<IEventHandler<TEvent>>();

            foreach (var handler in handlers)
            {
                if (!handler.IsApplicable(context, @event)) continue;

                handler.Handle(context, @event);
            }
        }
        

        public async Task PublishAsync<TEvent>(TEvent @event)
        {
            var context = _context.Value;

            var handlers = _sp.GetServices<IEventHandlerAsync<TEvent>>();

            var tasks = new List<Task>();

            foreach (var handler in handlers)
            {
                if (!handler.IsApplicable(context, @event)) continue;

                tasks.Add(handler.Handle(context, @event));
            }

            await Task.WhenAll(tasks);
        }
    }
}