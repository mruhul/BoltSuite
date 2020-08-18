using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Bolt.FluentHttpClient.Fakes
{
    public static class IocSetup
    {
        public static void AddFluentHttpClientFake(this IServiceCollection sc)
        {
            sc.TryAddSingleton<IFakeResponseProvider, FakeHttpResponseProvider>();
            sc.TryAddScoped<IHttpClientWrapper, FakeHttpClientWrapper>();
        }
    }
}
