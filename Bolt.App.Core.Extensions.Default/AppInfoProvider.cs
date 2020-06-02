using System;

namespace Bolt.App.Core.Extensions.Default
{
    /// <summary>
    /// Load settings from config providers for key {Bolt:AppInfo}.
    /// This is optional settings. if not available default value used.
    /// <example>
    ///    Sample settings to put in appsettings.json
    /// <code>
    /// {
    ///     "Bolt": {
    ///         "AppInfo": {
    ///             "Name": "api-notifications", // default AppDomain.CurrentDomain.FriendlyName with `.` replace with `-`
    ///             "Environment" : "production" //default is ASPNETCORE_ENVIRONMENT value
    ///         }
    ///     }
    /// }
    /// </code>
    /// </example>
    /// </summary>
    [ConfigSectionName("Bolt:AppInfo", isOptional: true)]
    public class AppInfoSettings
    {
        public string Name { get; set; } = string.Empty;
        public string Environment { get; set; } = string.Empty;
    }

    internal sealed class AppInfoProvider : IAppInfoProvider
    {
        private readonly IConfig<AppInfoSettings> _config;

        private static readonly string DefaultAppName = AppDomain.CurrentDomain
            .FriendlyName
            .ToLower()
            .Replace(".", "-");

        private static readonly string CurrentEnv = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        public AppInfoProvider(IConfig<AppInfoSettings> config)
        {
            _config = config;
        }

        public IAppInfo Get()
        {
            return new AppInfo
            {
                Environment = EmptyAlternative( _config.Value.Environment, CurrentEnv ?? string.Empty),
                Name = EmptyAlternative( _config.Value.Name,  DefaultAppName ?? string.Empty)
            };
        }

        private string EmptyAlternative(string value, string alternative)
        {
            return string.IsNullOrWhiteSpace(value)
                ? alternative
                : value;
        }
    }

    internal class AppInfo : IAppInfo
    {
        public string Name { get; set; }
        public string Environment { get; set; }
    }
}