using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Bolt.PubSub.RabbitMq
{
    public static class IocSetup
    {
        public static IServiceCollection AddRabbitMqPublisher(
            this IServiceCollection services,
            IConfiguration configuration,
            RabbitMqSetupOptions options = null)
        {
            options ??= new RabbitMqSetupOptions();

            var settings = new RabbitMqSettings();

            if(options.ConfigSectionName.HasValue())
            {
                configuration.GetSection(options.ConfigSectionName).Bind(settings);
            }

            services.AddLogging();
            services.TryAddSingleton<IUniqueId, UniqueId>();
            services.TryAddSingleton<IRabbitMqSettings, RabbitMqSettings>();
            services.TryAddEnumerable(ServiceDescriptor.Singleton<IMessageSerializer, JsonSerializer>());
            services.TryAddSingleton<RabbitMqConnection>();
            services.TryAddTransient<IMessagePublisher, Publishers.Publisher>();
            services.TryAddTransient<Publishers.IRabbitMqPublisher, Publishers.RabbitMqPublisher>();

            return services;
        }
    }

    public record RabbitMqSetupOptions
    {
        public string ConfigSectionName { get; init; } = "Bolt:PubSub:RabbitMq";
    }
}
