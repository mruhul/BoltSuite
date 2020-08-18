using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Xunit;
using Bolt.FluentHttpClient.Fluent;
using Bolt.FluentHttpClient.Fakes;
using Shouldly;

namespace Bolt.FluentHttpClient.Tests
{
    public class UnitTest1
    {
        private IServiceScope BuildScope()
        {
            var sc = new ServiceCollection();
            sc.AddFluentHttpClientFake();
            sc.AddDefaultFleuntHttpClient();

            var sp = sc.BuildServiceProvider();

            return sp.CreateScope();
        }

        [Fact]
        public async Task Fake_HttpClient_With_Match_Url()
        {
            using var scope = BuildScope();

            var httpClient = scope.ServiceProvider.GetRequiredService<IFluentHttpClient>();

            scope.FakeHttpClient()
                .Uri(x => x.AbsoluteUri == "http://www.google.com/")
                .RespondWith("Hello World");

            var googleMsg = await httpClient
                .ForUrl("http://www.google.com")
                .GetAsync<string>();

            googleMsg.Content.ShouldBe("Hello World");
        }

        [Fact]
        public async Task Fake_HttpClient_With_Match_Url_Is_Scoped()
        {
            using var scope = BuildScope();

            var httpClient = scope.ServiceProvider.GetRequiredService<IFluentHttpClient>();

            scope.FakeHttpClient()
                .Uri(x => x.AbsoluteUri == "http://www.google.com/")
                .HeaderContains("test","yes")
                .RespondWith("Hello World!");

            var googleMsg = await httpClient
                .ForUrl("http://www.google.com")
                .Header("test","yes")
                .GetAsync<string>();

            googleMsg.Content.ShouldBe("Hello World!");
        }

        [Fact]
        public async Task Test1()
        {
            using var scope = BuildScope();

            var httpClient = scope.ServiceProvider.GetRequiredService<IFluentHttpClient>();
            
            var rsp = await httpClient
                .ForUrl("https://ruhul.free.beeceptor.com/test")
                .TimeoutInSeconds(10)
                .Retry(1)
                .GetRequestAsync();

            var rsp1 = await httpClient
                .ForUrl("https://ruhul.free.beeceptor.com/test")
                .Timeout(TimeSpan.FromSeconds(10))
                .Retry(1)
                .GetRequestAsync();

            Assert.True(rsp.IsSuccessStatusCode);
        }
    }
}
