using System.Collections.Generic;
using System.Linq;
using Bolt.App.Core;

namespace Bolt.Tenancy
{
    public interface ITenantConfigSetup<T>
    {
        IDictionary<string, T> Get();
    }

    public abstract class TenantConfigSetup<T> : ITenantConfigSetup<T>
    {
        IDictionary<string, T> ITenantConfigSetup<T>.Get()
        {
            var valuesWithTenantName = this.Get();
            
            var result = new Dictionary<string,T>();

            if (valuesWithTenantName == null) return result;

            foreach (var valueTuple in valuesWithTenantName)
            {
                result[valueTuple.tenantName] = valueTuple.value;
            }
            
            return result;
        }

        protected abstract IEnumerable<(string tenantName, T value)> Get();
    }
}