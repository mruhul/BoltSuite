using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Bolt.FluentHttpClient
{
    public interface IHttpContentSerializer
    {
        Task SerializeAsync<T>(Stream stream, T value, CancellationToken cancellationToken);
        T Deserialize<T>(string value);
        ValueTask<T> DeserializeAsync<T>(Stream stream, CancellationToken cancellationToken);
        bool IsApplicable(string contentType);
    }
}
