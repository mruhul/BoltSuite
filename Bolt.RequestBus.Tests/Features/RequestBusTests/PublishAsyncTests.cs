using System.Collections.Generic;
using System.Threading.Tasks;
using Bolt.RequestBus.Tests.Infra;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Bolt.RequestBus.Tests.Features.RequestBusTests
{
    public class PublishAsyncTests
    {
        [Fact]
        public async Task Should_Execute_All_Applicable_EventHandlers()
        {
            var sut = IocHelper.GetRequestBus(sc =>
            {
                sc.AddTransient<IEventHandlerAsync<OrderPlaced>, OrderPlacedTest1EventHandler>();
                sc.AddTransient<IEventHandlerAsync<OrderPlaced>, OrderPlacedTest2EventHandler>();
                sc.AddTransient<IEventHandlerAsync<OrderPlaced>, OrderPlacedTest3EventHandler>();
            });

            var @event = new OrderPlaced();

            await sut.PublishAsync(@event);

            @event.HandlersProcessed.ShouldContain(nameof(OrderPlacedTest1EventHandler));
            @event.HandlersProcessed.ShouldContain(nameof(OrderPlacedTest2EventHandler));
            @event.HandlersProcessed.ShouldNotContain(nameof(OrderPlacedTest3EventHandler));
        }

        internal class OrderPlaced
        {
            public List<string> HandlersProcessed { get; set; } = new List<string>();
        }

        internal class OrderPlacedTest1EventHandler : EventHandlerAsync<OrderPlaced>
        {
            public override Task Handle(IRequestBusContext context, OrderPlaced @event)
            {
                @event.HandlersProcessed.Add(nameof(OrderPlacedTest1EventHandler));
                return Task.CompletedTask;
            }
        }

        internal class OrderPlacedTest2EventHandler : EventHandlerAsync<OrderPlaced>
        {
            public override Task Handle(IRequestBusContext context, OrderPlaced @event)
            {
                @event.HandlersProcessed.Add(nameof(OrderPlacedTest2EventHandler));
                return Task.CompletedTask;
            }
        }

        internal class OrderPlacedTest3EventHandler : EventHandlerAsync<OrderPlaced>
        {
            public override Task Handle(IRequestBusContext context, OrderPlaced @event)
            {
                @event.HandlersProcessed.Add(nameof(OrderPlacedTest3EventHandler));
                return Task.CompletedTask;
            }

            public override bool IsApplicable(IRequestBusContext context, OrderPlaced @event) => false;
        }
    }
}