using Bolt.App.Core;

namespace Bolt.Tenancy.Impl
{
    [ConfigSectionName("Bolt:Tenancy", isOptional: true)]
    internal class TenantDefaultConfigSettings
    {
        public CaseInsensitiveDictionary<None> Settings { get; set; }
    }
    
    internal class None {}
}