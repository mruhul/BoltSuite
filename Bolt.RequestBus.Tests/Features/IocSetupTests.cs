using System;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Bolt.RequestBus.Tests.Features
{
    public class IocSetupTests
    {
        [Fact]
        public void ShouldRegisterRequestBus()
        {
            var sc = new ServiceCollection();
            
            sc.AddRequestBus();
            
            var sp = sc.BuildServiceProvider();

            var bus = sp.GetService<IRequestBus>();

            bus.ShouldNotBeNull();
        }
    }
}