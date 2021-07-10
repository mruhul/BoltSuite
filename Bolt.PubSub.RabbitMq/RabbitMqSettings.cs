namespace Bolt.PubSub.RabbitMq
{
    internal interface IRabbitMqSettings
    {
        /// <summary>
        /// Amqp connection string to create rabbitmq connection. Must provided.
        /// </summary>
        string ConnectionString { get; }

        /// <summary>
        /// Name of the exchange to use
        /// </summary>
        string ExchangeName { get; }

        /// <summary>
        /// Set this true if exchange already exists or will be created manually. Default is false.
        /// </summary>
        bool SkipCreateExchange { get; }

        /// <summary>
        /// Set the exchange type. default is [headers]
        /// </summary>
        string ExchangeType { get; }

        /// <summary>
        /// Set the default content type of message. default is [application/json]
        /// </summary>
        string ContentType { get; }

        /// <summary>
        /// Prefix will be add with message type for [blt-msg-type] header if defined.
        /// </summary>
        string MessageTypePrefix { get; }

        /// <summary>
        /// If you like to set default expiry for all messages
        /// </summary>
        int? DefaultTTLInSeconds { get; }

        /// <summary>
        /// Application id used in header to set value for message type
        /// </summary>
        string AppId { get;}

        /// <summary>
        /// The lib will use this prefix for all headers used by lib itself
        /// </summary>
        string ImplicitHeaderPrefix { get; }
    }

    internal class RabbitMqSettings : IRabbitMqSettings
    {
        /// <summary>
        /// Amqp connection string to create rabbitmq connection. Must provided.
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// Name of the exchange to use
        /// </summary>
        public string ExchangeName { get; set; }

        /// <summary>
        /// Set this true if exchange already exists or will be created manually. Default is false.
        /// </summary>
        public bool SkipCreateExchange { get; set; } = false;

        /// <summary>
        /// Set the exchange type. default is [headers]
        /// </summary>
        public string ExchangeType { get; set; } = "headers";

        /// <summary>
        /// Set the default content type of message. default is [application/json]
        /// </summary>
        public string ContentType { get; set; } = ContentTypeNames.Json;

        /// <summary>
        /// Prefix will be add with message type for [blt-msg-type] header if defined.
        /// </summary>
        public string MessageTypePrefix { get; set; }

        /// <summary>
        /// If you like to set default expiry for all messages
        /// </summary>
        public int? DefaultTTLInSeconds { get; set; }

        public string AppId { get; set; }

        /// <summary>
        /// The lib will use this prefix for all headers used by lib itself.
        /// Don't use `x-` as prefix as rabbitmq has special usecase for them.
        /// </summary>
        public string ImplicitHeaderPrefix { get; set; } = "blt-";
    }
}
