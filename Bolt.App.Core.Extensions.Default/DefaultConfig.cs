using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Configuration;

namespace Bolt.App.Core.Extensions.Default
{
    internal sealed class DefaultConfig<T> : IConfig<T> where T : class, new()
    {
        private readonly Lazy<T> _settings;

        public DefaultConfig(IConfiguration config)
        {
            _settings = new Lazy<T>(() => LoadSettings(config));
        }
        
        public T Value => _settings.Value;
        
        private T LoadSettings(IConfiguration config)
        {
            var type = typeof(T);

            Log.Trace($"Start loading settings of type {type}");

            string sectionName;
            var isOptional = false;

            var attr = type.GetTypeInfo().GetCustomAttribute<ConfigSectionNameAttribute>();

            if (attr != null)
            {
                sectionName = attr.Name;
                isOptional = attr.IsOptional;
            }
            else
            {
                sectionName = $"Settings:{type.Name}";

                Log.Trace($"Settings type does not define any attribute {typeof(ConfigSectionNameAttribute)} so section name set to {sectionName}");
            }

            var exists = IsSectionExists(config, sectionName);

            var result = new T();

            if (!exists)
            {
                if (!isOptional)
                {
                    throw new Exception(
                        $"Failed to load settings for [{type.FullName}] from section name [{sectionName}]");
                }
                else
                {
                    Log.Info($"Section name [{sectionName}] does not exists. Using default settings instance");

                    return  result;
                }
            }

            config.GetSection(sectionName).Bind(result);

            return result;
        }

        private bool IsSectionExists(IConfiguration config, string sectionName)
        {
            return IsSectionExists(config.GetChildren(), sectionName);
        }

        private bool IsSectionExists(IEnumerable<IConfigurationSection> sections, string sectionName)
        {
            var firstColonPos = sectionName.IndexOf(":", StringComparison.Ordinal);

            if (firstColonPos == -1)
            {
                return sections.Any(x => x.Key == sectionName);
            }

            var fistPart = sectionName.Substring(0, firstColonPos);

            var section = sections.FirstOrDefault(x => x.Key == fistPart);

            return section != null && IsSectionExists(section.GetChildren(), sectionName.Substring(firstColonPos + 1));
        }
    }
}