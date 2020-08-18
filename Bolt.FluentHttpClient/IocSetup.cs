using Bolt.FluentHttpClient.Fluent;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Net.Http;

namespace Bolt.FluentHttpClient
{
    public class FluentHttpClientSetupOptions
    {
        public bool UseDefaultSerializer { get; set; } = true;
    }

    public static class IocSetup
    {
        public static IServiceCollection AddFluentHttpClient(this IServiceCollection sc, 
            FluentHttpClientSetupOptions options = null)
        {
            options = options ?? new FluentHttpClientSetupOptions();

            sc.AddLogging();
            sc.TryAddTransient<HttpRetryHandler>();
            sc.TryAddTransient<HttpTimeoutHandler>();
            sc.TryAddTransient<HttpRequestFilterHandler>();
            sc.TryAddTransient<IFluentHttpClient, FluentHttp>();

            if(options.UseDefaultSerializer)
            {
                sc.TryAddEnumerable(ServiceDescriptor.Singleton<IHttpContentSerializer, HttpContentJsonSerializer>());
            }

            sc.TryAddSingleton<IHttpClientWrapper, HttpClientWrapper>();

            return sc;
        }


        /// <summary>
        /// Setup default httpclient with all default message handlers.
        /// </summary>
        /// <param name="sc"></param>
        /// <param name="options"></param>
        public static void AddDefaultFleuntHttpClient(this IServiceCollection sc, FluentHttpClientSetupOptions options = null)
        {
            options = options ?? new FluentHttpClientSetupOptions();

            sc.TryAddTransient(x => x.GetService<IHttpClientFactory>().CreateClient("Default"));

            sc.AddFluentHttpClient(options)
                .AddHttpClient("Default")
                .AddDefaultHttpHandlers(options);
        }

        /// <summary>
        /// Add default handlers to your httpclient setup.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IHttpClientBuilder AddDefaultHttpHandlers(this IHttpClientBuilder builder, FluentHttpClientSetupOptions options = null)
        {
            options = options ?? new FluentHttpClientSetupOptions();

            var result = builder
                .AddHttpMessageHandler<HttpRequestFilterHandler>()
                .AddHttpMessageHandler<HttpRetryHandler>()
                .AddHttpMessageHandler<HttpTimeoutHandler>();

            return result;
        }
    }
}
