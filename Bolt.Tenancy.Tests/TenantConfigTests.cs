using System.Collections.Generic;
using System.Linq;
using Bolt.App.Core;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Bolt.Tenancy.Tests
{
    public class TenantConfigTests : IClassFixture<IocFixture>
    {
        private readonly IocFixture _fixture;

        public TenantConfigTests(IocFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public void Should_Able_To_Load_Settings_For_Multiple_Tenants_From_Configuration()
        {
            var config = _fixture.GetTenantConfig<EnvironmentBasedSettings>("bookworm-nz");
            config.Value.ServiceUrl.ShouldBe("http:/stg.auth.com.nz");
            config.Value.AdClickId.ShouldBe("123-nz");
            
        }

        [Fact]
        public void Should_Return_Null_When_No_Tenant_Provider_Available()
        {
            var config = _fixture.GetTenantConfig();
            config.CurrentTenantName.ShouldBeNull();
        }

        [Fact]
        public void Should_Return_Tenant_Name_Provided_By_Tenant_Provider()
        {
            var config = _fixture.GetTenantConfig("bookworm-bd");
            config.CurrentTenantName.ShouldBe("bookworm-bd");
        }

        [Fact]
        public void Should_Fallback_To_All_Providers_As_Long_Provider_Value_Is_Empty()
        {
            var config = _fixture.GetTenantConfig(null, sc =>
            {
                sc.AddTransient<ICurrentTenantNameProvider, TenantNameProviderSecond>();
                sc.AddTransient<ICurrentTenantNameProvider, TenantNameProviderFirst>();
                sc.AddTransient<ICurrentTenantNameProvider, TenantNameProviderThird>();
            });

            config.CurrentTenantName.ShouldBe("test-tenant");
        }

        [Fact]
        public void Should_Return_All_Supported_TenantNames()
        {
            var config = _fixture.GetTenantConfig();
            config.SupportedTenants.Count().ShouldBe(2);
            config.SupportedTenants.ShouldContain(x => x == "bookworm-au");
            config.SupportedTenants.ShouldContain(x => x == "bookworm-nz");
            config.SupportedTenants.ShouldNotContain(x => x == "bookworm-bd");
        }

        [Fact]
        public void Should_Return_Current_Tenant_Settings_From_ConfigurationFile()
        {
            var config = _fixture.GetTenantConfig<TestAppSettings>("bookworm-au");
            config.CurrentTenantName.ShouldBe("bookworm-au");
            config.Value.ShouldNotBeNull();
            config.Value.ExperienceApiBaseUrl.ShouldBe("http://api.com.au");
        }

        [Fact]
        public void Should_Return_Current_Tenant_Settings_From_Custom_Provider()
        {
            var config = _fixture.GetTenantConfig<TestInlineSettings>("bookworm-nz",
                sc => { sc.AddTransient<ITenantConfigSetup<TestInlineSettings>, CustomSettingsSetup>(); });
            config.CurrentTenantName.ShouldBe("bookworm-nz");
            config.Value.ShouldNotBeNull();
            config.Value.ExperienceApiBaseUrl.ShouldBe("http://local-api.com.nz");
        }

        [Fact]
        public void Should_Read_Settings_Only_Once()
        {
            var sp = _fixture.GetServiceProvider("bookworm-nz",
                sc => { sc.AddTransient<ITenantConfigSetup<TestExecutionCountSettings>, CustomExecutionSettingsProvider>(); });

            var config = sp.GetRequiredService<ITenantConfig<TestExecutionCountSettings>>();
            
            config.CurrentTenantName.ShouldBe("bookworm-nz");
            config.Value.ShouldNotBeNull();
            config.Value.Executed.ShouldBe(1);

            var config2 = sp.GetRequiredService<ITenantConfig<TestExecutionCountSettings>>();
            config2.CurrentTenantName.ShouldBe("bookworm-nz");
            config2.Value.ShouldNotBeNull();
            config2.Value.Executed.ShouldBe(1);
        }

        class TestExecutionCountSettings
        {
            public int Executed { get; set; } = -1;
        }

        class CustomExecutionSettingsProvider : TenantConfigSetup<TestExecutionCountSettings>
        {
            private static int Count = 0;
            protected override IEnumerable<(string tenantName, TestExecutionCountSettings value)> Get()
            {
                Count++;

                yield return ("bookworm-au", new TestExecutionCountSettings
                {
                    Executed = Count
                });
                
                yield return ("bookworm-nz", new TestExecutionCountSettings
                {
                    Executed = Count
                });
            }
        }

        class CustomSettingsSetup : TenantConfigSetup<TestInlineSettings>
        {
            protected override IEnumerable<(string tenantName, TestInlineSettings value)> Get()
            {
                yield return ("bookworm-au", new TestInlineSettings
                {
                    ExperienceApiBaseUrl = "http://local-api.com.au"
                });

                yield return ("bookworm-nz", new TestInlineSettings
                {
                    ExperienceApiBaseUrl = "http://local-api.com.nz"
                });
            }
        }

        class TestInlineSettings
        {
            public string ExperienceApiBaseUrl { get; set; }
        }

        [ConfigSectionName("Bolt:Tenancy")]
        class TestAppSettings
        {
            public string ExperienceApiBaseUrl { get; set; }
        }

        class TenantNameProviderFirst : ICurrentTenantNameProvider
        {
            public string Get()
            {
                return null;
            }

            public int Priority => 100;
        }

        class TenantNameProviderSecond : ICurrentTenantNameProvider
        {
            public string Get()
            {
                return string.Empty;
            }

            public int Priority => 90;
        }

        class TenantNameProviderThird : ICurrentTenantNameProvider
        {
            public string Get()
            {
                return "test-tenant";
            }

            public int Priority => 80;
        }

        /// <summary>
        /// Settings that generally change based on environment should load from appsettings or env settings
        /// </summary>
        [ConfigSectionName("TestApp:Core")] // that means the system will try to load settings for each tenant from "TestApp:Core" path
        class EnvironmentBasedSettings
        {
            public string ServiceUrl { get; set; }
            public string AdClickId { get; set; }
        }
    }
}