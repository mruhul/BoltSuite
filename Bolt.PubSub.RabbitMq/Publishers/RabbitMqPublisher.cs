using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Bolt.PubSub.RabbitMq.Tests")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
namespace Bolt.PubSub.RabbitMq.Publishers
{
    internal interface IRabbitMqPublisher : IDisposable
    {
        void Publish(PublishMessageWrapperDto dto);
    }

    internal sealed class RabbitMqPublisher : IRabbitMqPublisher
    {
        private readonly IModel channel;
        private readonly ILogger<RabbitMqLogger> logger;

        public RabbitMqPublisher(RabbitMqConnection connection, ILogger<RabbitMqLogger> logger)
        {
            this.logger = logger;

            logger.LogTrace("Start creating rabbitmq channel");

            channel = connection.GetOrCreate().CreateModel();
        }

        public void Dispose()
        {
            logger.LogTrace("Disposing channel");

            channel?.Dispose();
        }

        public void Publish(PublishMessageWrapperDto dto)
        {
            logger.LogTrace("Start publishing message");

            var properties = channel.CreateBasicProperties();

            properties.MessageId = dto.MessageId.ToString();
            properties.CorrelationId = dto.CorrelationId;
            properties.ContentType = dto.ContentType;

            if (properties.Headers is null) 
                properties.Headers = new Dictionary<string, object>(); 

            if (dto.Headers is not null)
            {
                foreach (var header in dto.Headers)
                {
                    properties.Headers[header.Key] = header.Value;
                }
            }

            if (dto.ExpiryInSeconds.HasValue)
            {
                logger.LogTrace("Setting up expiry of the message to {@expiryInSeconds}", dto.ExpiryInSeconds);

                properties.Expiration = dto.ExpiryInSeconds.Value.ToString();
            }

            logger.LogTrace("Setting delivery mode to {@deliveryMode}", dto.DeliveryMode);

            properties.DeliveryMode = dto.DeliveryMode;

            logger.LogTrace("Publishing message to {exchange} with {routingKey}", dto.Exchange, dto.RoutingKey);

            channel.BasicPublish(dto.Exchange, dto.RoutingKey, false, properties, dto.Content);

            logger.LogTrace("Message published successfully");
        }
    }

    public record PublishMessageWrapperDto
    {
        public Guid MessageId { get; init; }
        public string CorrelationId { get; init; }
        public Dictionary<string, string> Headers { get; init; }
        public string ContentType { get; init; }
        public string Exchange { get; init; }
        public string RoutingKey { get; init; }
        public byte DeliveryMode { get; init; } = 2;
        public int? ExpiryInSeconds { get; init; }
        public ReadOnlyMemory<byte> Content { get; init; }
    }
}
