using System.Collections.Generic;
using Bolt.RequestBus.Tests.Infra;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Bolt.RequestBus.Tests
{
    public class RequestBusPublishTests
    {
        [Fact]
        public void Publish_Should_Execute_All_Applicable_EventHandlers()
        {
            var sut = IocHelper.GetRequestBus(sc =>
            {
                sc.AddTransient<IEventHandler<OrderPlaced>, OrderPlacedTest1EventHandler>();
                sc.AddTransient<IEventHandler<OrderPlaced>, OrderPlacedTest2EventHandler>();
                sc.AddTransient<IEventHandler<OrderPlaced>, OrderPlacedTest3EventHandler>();
            });
            
            var @event = new OrderPlaced();
            
            sut.Publish(@event);
            
            @event.HandlerProcessed.ShouldContain(nameof(OrderPlacedTest1EventHandler));
            @event.HandlerProcessed.ShouldContain(nameof(OrderPlacedTest2EventHandler));
            @event.HandlerProcessed.ShouldNotContain(nameof(OrderPlacedTest3EventHandler));
        }
        
        internal class OrderPlaced
        {
            public List<string> HandlerProcessed { get; set; } = new List<string>();
        }
        
        internal class OrderPlacedTest1EventHandler : EventHandler<OrderPlaced>
        {
            public override void Handle(IRequestBusContext context, OrderPlaced @event)
            {
                @event.HandlerProcessed.Add(nameof(OrderPlacedTest1EventHandler));
            }
        }
        
        internal class OrderPlacedTest2EventHandler : EventHandler<OrderPlaced>
        {
            public override void Handle(IRequestBusContext context, OrderPlaced @event)
            {
                @event.HandlerProcessed.Add(nameof(OrderPlacedTest2EventHandler));
            }
        }
        
        internal class OrderPlacedTest3EventHandler : EventHandler<OrderPlaced>
        {
            public override void Handle(IRequestBusContext context, OrderPlaced @event)
            {
                @event.HandlerProcessed.Add(nameof(OrderPlacedTest3EventHandler));
            }

            public override bool IsApplicable(IRequestBusContext context, OrderPlaced @event) => false;
        }
    }
}