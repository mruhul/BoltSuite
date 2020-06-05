using System;
using Bolt.App.Core.Extensions.Default;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Bolt.Tenancy.Tests
{
    public class IocFixture
    {
        private IServiceProvider BuildServiceProvider(Action<IServiceCollection> action, string tenantName)
        {
            var sc = new ServiceCollection();
         
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            sc.AddSingleton<IConfiguration>(config);
            sc.AddCoreFeatures();
            sc.AddTenancy();
            
            sc.AddTransient<ICurrentTenantNameProvider>(sc => new StaticTenantNameProvider(tenantName));
            
            action?.Invoke(sc);
            
            return sc.BuildServiceProvider();
        }
        
        public ITenantConfig GetTenantConfig(string currentTenant = null, Action<IServiceCollection> action = null) => BuildServiceProvider(action, currentTenant).GetRequiredService<ITenantConfig>();
        public ITenantConfig<T> GetTenantConfig<T>(string currentTenant = null, Action<IServiceCollection> action = null) => BuildServiceProvider(action, currentTenant).GetRequiredService<ITenantConfig<T>>();
        public T GetService<T>(string currentTenant = null, Action<IServiceCollection> action = null) => BuildServiceProvider( action,currentTenant).GetRequiredService<T>();
        public IServiceProvider GetServiceProvider(string currentTenant = null, Action<IServiceCollection> action = null) => BuildServiceProvider( action,currentTenant);
        
        class StaticTenantNameProvider : ICurrentTenantNameProvider
        {
            private readonly string _name;

            public StaticTenantNameProvider(string name)
            {
                _name = name;
            }
            
            public string Get()
            {
                return _name;
            }

            public int Priority { get; } = int.MaxValue;
        }
    }
}