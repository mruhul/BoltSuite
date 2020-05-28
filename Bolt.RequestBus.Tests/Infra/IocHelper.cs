using System;
using Microsoft.Extensions.DependencyInjection;

namespace Bolt.RequestBus.Tests.Infra
{
    public static class IocHelper
    {
        public static IServiceCollection ServiceCollection()
        {
            var sc = new ServiceCollection();
            
            sc.AddRequestBus();

            return sc;
        }

        public static IRequestBus GetRequestBus(Action<IServiceCollection> register = null)
        {
            var sc = ServiceCollection();
            register?.Invoke(sc);
            var sp = sc.BuildServiceProvider();
            return sp.GetRequiredService<IRequestBus>();
        }
    }
}