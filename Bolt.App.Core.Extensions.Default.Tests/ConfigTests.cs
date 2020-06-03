using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Shouldly;
using Xunit;

namespace Bolt.App.Core.Extensions.Default.Tests
{
    public class ConfigTests
    {
        [Fact]
        public void Should_Load_Settings_From_Configuration()
        {
            var settings = GetConfig<TestSettings>();
            
            settings.Value.ShouldNotBeNull();
            settings.Value.Name.ShouldBe("testname");
            settings.Value.IsEnabled.ShouldBe(true);
            settings.Value.CoreApiUrl.ShouldBe("http://core-api.com/hello/");
            settings.Value.OptionalValue.ShouldBe("DefaultValue");
        }

        [Fact]
        public void Should_Load_Default_Dto_For_NonExisting_Settings_When_Attribute_Set_To_Optional_True()
        {
            var settings = GetConfig<NonExistingOptionalSettings>();
            settings.Value.Name.ShouldBe("Default");
            settings.Value.Value.ShouldBeNull();
        }

        [Fact]
        public void Should_Throw_Exception_For_NonExisting_Settings_When_Attribute_Set_To_Optional_False()
        {
            var settings = GetConfig<NonExistingRequiredSettings>();
            Should.Throw<Exception>(() => settings.Value);
        }
        
        [ConfigSectionName("TestApp:Settings")]
        class TestSettings
        {
            public string Name { get; set; }
            public bool IsEnabled { get; set; }
            public string CoreApiUrl { get; set; }
            public string OptionalValue { get; set; } = "DefaultValue";
        }

        [ConfigSectionName("NonExisting", isOptional: true)]
        class NonExistingOptionalSettings
        {
            public string Name { get; set; } = "Default";
            public string Value { get; set; }
        }
        
        [ConfigSectionName("NonExisting", isOptional: false)]
        class NonExistingRequiredSettings
        {
            
        }
        
        private IConfig<T> GetConfig<T>() where T: class, new()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
            
            var sc = new ServiceCollection();
            sc.TryAddSingleton<IConfiguration>(config);
            sc.AddCoreFeatures();
            
            return sc.BuildServiceProvider().GetRequiredService<IConfig<T>>();
        }
    }
}