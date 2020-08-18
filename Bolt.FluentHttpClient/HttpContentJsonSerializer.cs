using Newtonsoft.Json;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Bolt.FluentHttpClient
{
    public class HttpContentJsonSerializer : IHttpContentSerializer
    {
        public T Deserialize<T>(string value)
        {
            if (value == null) return default;

            return JsonConvert.DeserializeObject<T>(value);
        }

        public ValueTask<T> DeserializeAsync<T>(Stream stream, CancellationToken cancellationToken)
        {
            var serializer = new JsonSerializer();
            
            using (var sr = new StreamReader(stream))
            
            using (var jsonTextReader = new JsonTextReader(sr))
            {
                var value = serializer.Deserialize<T>(jsonTextReader);

                return new ValueTask<T>(value);
            }
        }

        public bool IsApplicable(string contentType)
        {
            return contentType.IndexOf("json") != -1;
        }

        public Task SerializeAsync<T>(Stream stream, T value, CancellationToken cancellationToken)
        {
            using (StreamWriter writer = new StreamWriter(stream))
            using (JsonTextWriter jsonWriter = new JsonTextWriter(writer))
            {
                JsonSerializer ser = new JsonSerializer();
                ser.Serialize(jsonWriter, value);
                jsonWriter.Flush();
            }

            return Task.CompletedTask;
        }
    }
}
