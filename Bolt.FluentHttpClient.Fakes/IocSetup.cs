using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Bolt.FluentHttpClient.Fakes
{
    public static class IocSetup
    {
        /// <summary>
        /// Setup httpclient to fake response per scope. this is good option if you create scope from your 
        /// service provider for each test to isolate the test setup for each test. Use asSingleton when
        /// you use this for webserver base test
        /// </summary>
        /// <param name="sc"></param>
        /// <param name="asSingleton"></param>
        public static void AddFakeHttpClientWrapper(this IServiceCollection sc, bool asSingleton = false)
        {
            if (asSingleton)
            {
                sc.TryAddSingleton<IFakeResponseProvider, FakeHttpResponseProvider>();
                sc.TryAddSingleton<IHttpClientWrapper, FakeHttpClientWrapper>();

                sc.Replace(ServiceDescriptor.Singleton<IFakeResponseProvider, FakeHttpResponseProvider>());
                sc.Replace(ServiceDescriptor.Singleton<IHttpClientWrapper, FakeHttpClientWrapper>());
            }
            else
            {
                sc.TryAddScoped<IFakeResponseProvider, FakeHttpResponseProvider>();
                sc.TryAddScoped<IHttpClientWrapper, FakeHttpClientWrapper>();

                sc.Replace(ServiceDescriptor.Scoped<IFakeResponseProvider, FakeHttpResponseProvider>());
                sc.Replace(ServiceDescriptor.Scoped<IHttpClientWrapper, FakeHttpClientWrapper>());
            }
        }
    }
}
