using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Bolt.PubSub.RabbitMq
{
    internal sealed class JsonSerializer : IMessageSerializer
    {
        private readonly JsonSerializerOptions options;

        public JsonSerializer()
        {
            options = new JsonSerializerOptions();
            options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.PropertyNameCaseInsensitive = true;
            options.Converters.Add(new JsonStringEnumConverter());
            options.Converters.Add(new DateTimeConverter());
            options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        }

        public T Deserialize<T>(ReadOnlySpan<byte> content)
        {
            return content == null || content.Length == 0 
                ? default 
                : System.Text.Json.JsonSerializer.Deserialize<T>(content, options);
        }

        public bool IsApplicable(string contentType)
        {
            return contentType.IsSame(ContentTypeNames.Json, 
                "application/javascript", 
                "text/json");
        }

        public byte[] Serialize<T>(T content)
        {
            return content == null 
                ? null 
                : System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(content, options);
        }
    }
}
