using Bolt.Tenancy.Impl;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Bolt.Tenancy
{
    public static class IocSetup
    {
        public static void AddTenancy(this IServiceCollection services)
        {
            services.AddLogging();
            services.TryAddScoped<ITenantConfig, TenantConfig>();
            services.TryAdd(ServiceDescriptor.Scoped(typeof(ITenantConfig<>), typeof(TenantConfig<>)));
            services.TryAddSingleton<ConfigSectionReader>();
            services.TryAdd(ServiceDescriptor.Singleton(typeof(IConfigAggregator<>), typeof(ConfigAggregator<>)));
        }
    }
}