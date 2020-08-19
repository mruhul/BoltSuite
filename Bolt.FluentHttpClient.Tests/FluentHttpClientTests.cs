using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Xunit;
using Bolt.FluentHttpClient.Fluent;

namespace Bolt.FluentHttpClient.Tests
{
    public class FluentHttpClientTests
    {
        private IServiceScope BuildScope()
        {
            var sc = new ServiceCollection();

            sc.AddDefaultFleuntHttpClient();

            var sp = sc.BuildServiceProvider();

            return sp.CreateScope();
        }

        [Fact]
        public async Task GetAsync_Should_Return_Response()
        {
            using var scope = BuildScope();

            var httpClient = scope.ServiceProvider.GetRequiredService<IFluentHttpClient>();

            var rsp = await httpClient
                .ForUrl("https://ruhul.free.beeceptor.com/test")
                .TimeoutInSeconds(10)
                .Retry(1)
                .GetRequestAsync();

            Assert.True(rsp.IsSuccessStatusCode);
        }
    }
}
