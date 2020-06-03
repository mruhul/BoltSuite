using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Bolt.App.Core.Extensions.Default.Tests
{
    public class AppInfoProviderTests
    {
        [Fact]
        public void Should_Load_Default_AppInfo_When_No_Settings_Provided()
        {
            var config = new ConfigurationBuilder().Build();

            var result = GetAppInfo();
            
            result.ShouldNotBeNull();
            result.Name.ShouldBe("testhost"); // This is what current assembly name when running under xunit runtime
            result.Environment.ShouldBe(string.Empty); // we didn't set any aspnetcore_env variable
        }

        [Fact]
        public void Should_Load_AppInfo_From_AppSettings_Json_File()
        {
            var result = GetAppInfo("appsettings.json");
            
            result.ShouldNotBeNull();
            result.Name.ShouldBe("app-test");
            result.Environment.ShouldBe("test");
        }
        
        private IAppInfo GetAppInfo(string appSettingsFileName = null)
        {
            var configBuilder = new ConfigurationBuilder();

            if (!string.IsNullOrWhiteSpace(appSettingsFileName))
            {
                configBuilder.AddJsonFile(appSettingsFileName);
            }
            
            var sc = new ServiceCollection();
            
            sc.AddSingleton<IConfiguration>(configBuilder.Build());
            sc.AddCoreFeatures();
            
            var sp = sc.BuildServiceProvider();
            var appInfo = sp.GetRequiredService<IAppInfoProvider>();
            return appInfo.Get();
        }
    }
}