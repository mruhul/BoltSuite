using System;

namespace Bolt.App.Core
{
    public interface IConfig<out T> where T : class, new()
    {
        T Value { get; }
    }

    /// <summary>
    /// Attribute class to use with Config dto to instruct <see cref="IConfig{T}"/> where from appsettings
    /// we can load the value. If IsOptional is false then an exception will be thrown when bo settings define.
    ///
    /// <example>
    ///     <code>
    ///     [ConfigSectionName("MyApp:SearchApiSettings", false)]
    ///     public class SearchApiSettings
    ///     {
    ///         public string BaseUrl { get; set; }
    ///     }
    ///
    ///     public class SearchApiService : ISearchApiService
    ///     {
    ///         private readonly SearchApiSettings _settings;
    /// 
    ///         public SearchApiService(IConfig{SearchApiSettings} settings)
    ///         {
    ///             _settings = settings.Value;
    ///         }
    ///     }
    ///     </code>     
    /// </example>
    /// </summary>
    public class ConfigSectionNameAttribute : Attribute
    {
        public ConfigSectionNameAttribute(string name, bool isOptional = false)
        {
            Name = name;
            IsOptional = isOptional;
        }
        
        public string Name { get; private set; }
        public bool IsOptional { get; private set; }
    }
}