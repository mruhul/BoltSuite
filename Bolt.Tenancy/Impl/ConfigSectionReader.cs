using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Bolt.App.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Bolt.Tenancy.Impl
{
    internal sealed class ConfigSectionReader
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<ConfigSectionReader> _logger;

        public ConfigSectionReader(IConfiguration configuration, ILogger<ConfigSectionReader> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }
        
        public TenantConfigAppSettingsDto<T> Read<T>()
        {
            var type = typeof(T);

            _logger.LogTrace($"Start loading settings of type {type}");

            string sectionName;
            var isOptional = true;

            var attr = type.GetTypeInfo().GetCustomAttribute<ConfigSectionNameAttribute>();

            if (attr != null)
            {
                sectionName = attr.Name;
                isOptional = attr.IsOptional;
            }
            else
            {
                return new TenantConfigAppSettingsDto<T>();
            }

            var exists = IsSectionExists(_configuration, sectionName);

            var result = new TenantConfigAppSettingsDto<T>();

            if (!exists)
            {
                if (!isOptional)
                {
                    throw new Exception(
                        $"Failed to load settings for [{type.FullName}] from section name [{sectionName}]");
                }

                _logger.LogInformation($"Section name [{sectionName}] does not exists. Using default settings instance");

                return  result;
            }

            _configuration.GetSection(sectionName).Bind(result);

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

    internal sealed class TenantConfigAppSettingsDto<T>
    {
        public CaseInsensitiveDictionary<T> Settings { get; set; }
    }

    internal static class Constants
    {
        public const string SectionName = "Bolt:Tenancy";
    }
}