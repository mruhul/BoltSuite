using Microsoft.Extensions.DependencyInjection;

namespace Bolt.FluentHttpClient.Tests
{
    public class HttpClientFixture
    {
        public HttpClientFixture()
        {
            var sc = new ServiceCollection();

            sc.AddFluentHttpClient()
                .AddDefaultHttpHandlers();

            var sp = sc.BuildServiceProvider();

            HttpClient = sp.GetRequiredService<IFluentHttpClient>();
        }

        public IFluentHttpClient HttpClient { get; private set; }
    }
}
