namespace Bolt.App.Core
{
    public interface IAppInfoProvider
    {
        /// <summary>
        /// Provide application information e.g name and environment
        /// </summary>
        /// <returns>IAppInfo</returns>
        IAppInfo Get();
    }

    public interface IAppInfo
    {
        public string Name { get; }
        public string Environment { get; }
    }
}