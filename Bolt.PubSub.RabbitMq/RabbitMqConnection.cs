using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System;


namespace Bolt.PubSub.RabbitMq
{
    internal sealed class RabbitMqConnection : IDisposable
    {
        private readonly Lazy<IConnection> connection;
        private readonly IRabbitMqSettings settings;
        private readonly ILogger<RabbitMqLogger> logger;

        public RabbitMqConnection(IRabbitMqSettings settings, ILogger<RabbitMqLogger> logger)
        {
            this.settings = settings;
            this.logger = logger;
            connection = new Lazy<IConnection>(() => CreateConnection());
        }

        private IConnection CreateConnection()
        {
            if (settings == null) throw new ArgumentException($"{nameof(settings)} cannot be null.");
            if (settings.ConnectionString.IsEmpty()) throw new ArgumentException($"{nameof(settings.ConnectionString)} cannot be null or empty.");
            if (settings.ExchangeName.IsEmpty()) throw new ArgumentNullException($"{nameof(settings.ExchangeName)} cannot be null or empty.");

            logger.LogDebug("Start creating rabbitmq connection.");

            var connectionFactory = new ConnectionFactory
            {
                Uri = new Uri(settings.ConnectionString)
            };

            var con = connectionFactory.CreateConnection();

            logger.LogDebug("Rabbitmq connection created successfully.");

            if (settings.SkipCreateExchange is true) return con;

            var exchangeType = settings.ExchangeType.EmptyAlternative("headers");

            logger.LogDebug("Start creating {exchangeName} of {exchangeType}.", settings.ExchangeName, exchangeType);

            using var channel = con.CreateModel();

            channel.ExchangeDeclare(settings.ExchangeName,
                exchangeType,
                true,
                false);

            logger.LogDebug("{exchangeName} of {exchangeType} created successfully", settings.ExchangeName, exchangeType);

            return con;
        }

        public IConnection GetOrCreate()
        {
            return connection.Value;
        }

        public void Dispose()
        {
            if (connection.IsValueCreated)
            {
                logger.LogDebug("Disposing rabbitmq connection");

                connection.Value?.Dispose();
            }
        }
    }
}
