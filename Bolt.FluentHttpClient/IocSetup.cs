﻿using Bolt.FluentHttpClient.Fluent;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Net.Http;

namespace Bolt.FluentHttpClient
{
    public class FluentHttpClientSetupOptions
    {
        public bool UseDefaultSerializer { get; set; } = true;
        public HttpContentSerializerSettings HttpContentSerializerSettings { get; set; }
    }

    public static class IocSetup
    {
        private const string defaultHttpClientName = "Bolt.DefaultClient";

        public static IHttpClientBuilder AddFluentHttpClient(this IServiceCollection sc, 
            FluentHttpClientSetupOptions options = null)
        {
            options = options ?? new FluentHttpClientSetupOptions();

            sc.AddLogging();
            sc.TryAddTransient<HttpRetryHandler>();
            sc.TryAddTransient<HttpTimeoutHandler>();
            sc.TryAddTransient<IFluentHttpClient, FluentHttp>();
            sc.TryAddTransient<ITypedHttpClient, TypedHttpClient>();
            
            

            if(options.UseDefaultSerializer)
            {
                sc.AddSingleton(options.HttpContentSerializerSettings ?? new HttpContentSerializerSettings());
                sc.TryAddEnumerable(ServiceDescriptor.Singleton<IHttpContentSerializer, HttpContentJsonSerializer>());
            }

            sc.TryAddTransient(x => x.GetService<IHttpClientFactory>().CreateClient(defaultHttpClientName));

            sc.TryAddTransient<IHttpClientWrapper, HttpClientWrapper>();
            sc.TryAddTransient<HttpClientRequestSender>();

            return sc.AddHttpClient(defaultHttpClientName);
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
                .AddHttpMessageHandler<HttpRetryHandler>()
                .AddHttpMessageHandler<HttpTimeoutHandler>();

            return result;
        }
    }
}
