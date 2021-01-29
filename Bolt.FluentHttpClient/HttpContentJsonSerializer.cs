using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Bolt.FluentHttpClient
{
    public class HttpContentSerializerSettings
    {
        public bool UseCamelCaseNaming { get; set; }
        public bool ProcessDictionaryKeys { get; set; }
        public bool IgnoreNull { get; set; }
        public bool UseStringEnumConverter { get; set; }
        public bool Indented { get; set; }
        public bool UtcDateTimeZoneHandling { get; set; }
    }

    internal sealed class HttpContentJsonSerializer : IHttpContentSerializer
    {
        private readonly JsonSerializerSettings settings = new JsonSerializerSettings();

        public HttpContentJsonSerializer(HttpContentSerializerSettings settings)
        {
            this.settings.Formatting = settings.Indented ? Formatting.Indented : Formatting.None;

            if (settings.UseCamelCaseNaming)
            {
                this.settings.ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy(processDictionaryKeys: settings.ProcessDictionaryKeys, overrideSpecifiedNames: true)
                };
            }

            if(settings.UtcDateTimeZoneHandling)
            {
                this.settings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
            }

            this.settings.NullValueHandling = settings.IgnoreNull ? NullValueHandling.Ignore : NullValueHandling.Include;
            
            if(settings.UseStringEnumConverter)
            {
                this.settings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
            }
        }

        public T Deserialize<T>(string value)
        {
            if (value == null) return default;

            return JsonConvert.DeserializeObject<T>(value);
        }

        public ValueTask<T> DeserializeAsync<T>(Stream stream, CancellationToken cancellationToken)
        {
            if (stream == null) return new ValueTask<T>(default(T));

            using var reader = new StreamReader(stream);
            using var jsonReader = new JsonTextReader(reader);
            var serializer = JsonSerializer.Create();
            return new ValueTask<T>(serializer.Deserialize<T>(jsonReader));
        }

        public bool IsApplicable(string contentType)
        {
            return contentType.IndexOf("json") != -1;
        }

        public async Task SerializeAsync<T>(Stream stream, T value, CancellationToken cancellationToken)
        {
            using (var sw = new StreamWriter(stream, new UTF8Encoding(false), 1024, true))
            {
                using var jw = new JsonTextWriter(sw) { Formatting = Formatting.None };
                JsonSerializer.Create(settings).Serialize(jw, value);
                await jw.FlushAsync(cancellationToken).ConfigureAwait(false);
            }

            stream.Seek(0, SeekOrigin.Begin);
        }
    }
}
