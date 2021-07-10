using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bolt.PubSub.RabbitMq.Tests
{
    public static class JsonExtensions
    {
        public static string ToJson(this object source)
            =>  source == null ? null : System.Text.Json.JsonSerializer.Serialize(source, new System.Text.Json.JsonSerializerOptions { 
            
                WriteIndented = true,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull  ,
                PropertyNameCaseInsensitive = false,
                PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
            });

        public static T FromJson<T>(this string source)
            => string.IsNullOrWhiteSpace(source) ? default : System.Text.Json.JsonSerializer.Deserialize<T>(source);

        public static T FromJson<T>(this ReadOnlyMemory<byte> source)
            => System.Text.Json.JsonSerializer.Deserialize<T>(source.ToArray());
    }
}
