using Bolt.App.Core;

namespace Bolt.Tenancy
{
    public interface ITenantConfig
    {
        string CurrentTenantName { get; }
        string[] SupportedTenants { get; }
        bool IsTenantSupported(string tenantName);
    }

    public interface ITenantConfig<out T> : ITenantConfig
    {
        T Value { get; }
    }
}