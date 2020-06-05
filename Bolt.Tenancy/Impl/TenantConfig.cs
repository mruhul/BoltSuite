using System;
using System.Collections.Generic;
using System.Linq;
using Bolt.App.Core;

namespace Bolt.Tenancy.Impl
{
    internal sealed class TenantConfig : ITenantConfig
    {
        private readonly IEnumerable<ICurrentTenantNameProvider> _providers;
        private readonly CaseInsensitiveDictionary<None> _config;
        private readonly Lazy<string> _lazy;

        public TenantConfig(IEnumerable<ICurrentTenantNameProvider> providers, IConfig<TenantDefaultConfigSettings> config)
        {
            _providers = providers;
            _config = config.Value?.Settings ?? new CaseInsensitiveDictionary<None>();
            _lazy = new Lazy<string>(GetCurrentTenantName);
        }

        private string GetCurrentTenantName()
        {
            foreach (var nameProvider in _providers.OrderByDescending(x => x.Priority))
            {
                var result = nameProvider.Get();
                
                if(string.IsNullOrWhiteSpace(result)) continue;

                return result;
            }

            return null;
        }

        public string CurrentTenantName => _lazy.Value;
        public string[] SupportedTenants => _config.Keys.ToArray();
        public bool IsTenantSupported(string tenantName) => _config.ContainsKey(tenantName);
    }
    
    internal sealed class TenantConfig<T> : ITenantConfig<T>
    {
        private readonly ITenantConfig _tenantConfig;
        private readonly CaseInsensitiveDictionary<T> _data;

        public TenantConfig(ITenantConfig tenantConfig, IConfigAggregator<T> aggregator)
        {
            _tenantConfig = tenantConfig;
            _data = aggregator.Get();
        }

        public string CurrentTenantName => _tenantConfig.CurrentTenantName;
        public string[] SupportedTenants => _tenantConfig.SupportedTenants;
        public bool IsTenantSupported(string tenantName) => _tenantConfig.IsTenantSupported(tenantName);

        public T Value => _data.TryGetValue(_tenantConfig.CurrentTenantName, out var value) ? value : default;
    }
}