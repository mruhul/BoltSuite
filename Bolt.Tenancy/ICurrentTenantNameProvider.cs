namespace Bolt.Tenancy
{
    public interface ICurrentTenantNameProvider
    {
        /// <summary>
        /// Return current tenant name. if return value is null or empty then it goes to next provider with lower priority
        /// </summary>
        /// <returns>string</returns>
        string Get();
        
        /// <summary>
        /// Higher value will get chance to execute fast and decide first.
        /// </summary>
        int Priority { get; }
    }
}