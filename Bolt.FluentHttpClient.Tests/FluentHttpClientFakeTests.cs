using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Xunit;
using Bolt.FluentHttpClient.Fluent;
using Bolt.FluentHttpClient.Fakes;
using Shouldly;
using System.Net;

namespace Bolt.FluentHttpClient.Tests
{
    public class FluentHttpClientFakeTests
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
                .WhenUri(x => x.AbsoluteUri == "http://www.google.com/")
                .RespondOk("Hello World");

            var googleMsg = await httpClient
                .ForUrl("http://www.google.com")
                .GetAsync<string>();

            googleMsg.StatusCode.ShouldBe(HttpStatusCode.OK);
            googleMsg.Content.ShouldBe("Hello World");
        }

        [Fact]
        public async Task Fake_HttpClient_With_Match_Url_Is_Scoped()
        {
            using var scope = BuildScope();

            var httpClient = scope.ServiceProvider.GetRequiredService<IFluentHttpClient>();

            scope.FakeHttpClient()
                .WhenUri(x => x.AbsoluteUri == "http://www.google.com/")
                .WhenHeaderContains("test","yes")
                .RespondWith(HttpStatusCode.OK, "Hello World!");

            var googleMsg = await httpClient
                .ForUrl("http://www.google.com")
                .Header("test","yes")
                .GetAsync<string>();

            googleMsg.Content.ShouldBe("Hello World!");
        }

        [Fact]
        public async Task Fake_HttpClient_Should_Return_Fake_StatusCode_When_Match()
        {
            using var scope = BuildScope();

            scope.FakeHttpClient()
                .WhenUri(x => x.AbsoluteUri == "http://www.google.com/bad-request")
                .RespondWith(HttpStatusCode.BadRequest);

            var httpClient = scope.ServiceProvider.GetRequiredService<IFluentHttpClient>();

            var rsp = await httpClient
                .ForUrl("http://www.google.com/bad-request")
                .GetAsync();

            rsp.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        }
    }
}
