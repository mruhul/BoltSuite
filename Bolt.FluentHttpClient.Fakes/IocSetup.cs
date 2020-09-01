using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Bolt.FluentHttpClient.Fakes
{
    public static class IocSetup
    {
        /// <summary>
        /// Setup httpclient to fake response per scope. this is good option if you create scope from your 
        /// service provider for each test to isolate the test setup for each test.
        /// </summary>
        /// <param name="sc"></param>
        public static void AddFakeHttpClientWrapper(this IServiceCollection sc)
        {
            sc.TryAddScoped<IFakeResponseProvider, FakeHttpResponseProvider>();
            sc.TryAddScoped<IHttpClientWrapper, FakeHttpClientWrapper>();

            sc.Replace(ServiceDescriptor.Scoped<IFakeResponseProvider, FakeHttpResponseProvider>());
            sc.Replace(ServiceDescriptor.Scoped<IHttpClientWrapper, FakeHttpClientWrapper>());
        }

        /// <summary>
        /// This method append a handler that fake the httpclient request if any fake setup available in IFakeResponseProvider
        /// As we don't have control on scope of httpmessagehandler creation so this option is better suited for in memory 
        /// server based test where you have singleton fake setup that shared accrosss all tests
        /// </summary>
        /// <param name="builder"></param>
        public static void AddFakeHttpHandler(this IHttpClientBuilder builder)
        {
            builder.Services.TryAddSingleton<IFakeResponseProvider, FakeHttpResponseProvider>();
            builder.Services.Replace(ServiceDescriptor.Singleton<IFakeResponseProvider, FakeHttpResponseProvider>());
            builder.Services.TryAddTransient<FakeHttpMessageHandler>();
            builder.AddHttpMessageHandler<FakeHttpMessageHandler>();
        }
    }

    public class FluentHttpClientFakeOption
    {
        /// <summary>
        /// You can setup fake for your httpclient request in two way. One is replace existing httplcientwrapper
        /// with a fakehttpclientwrapper impl that provided part of this solution. This allow you to fake per scope.
        /// Or other option is use fakehttpmessagehandler which required you attach a httpmessagehandler part of your
        /// httpclient setup. httpmessagehandler provide fake at the very last level fake but doesn't honour setup per scope
        /// </summary>
        public bool ExcludeFakeHttpClientWrapper { get; set; }
    }
}
