using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Bolt.RequestBus
{
    public static class IocSetup
    {
        public static void AddRequestBus(this IServiceCollection sc)
        {
            sc.TryAdd(ServiceDescriptor.Scoped<IRequestBus,Impl.RequestBus>());
            sc.AddLogging();
        }
    }
}