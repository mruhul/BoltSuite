using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Bolt.FluentHttpClient.Fakes
{
    internal sealed class FakeHttpClientWrapper : IHttpClientWrapper
    {
        private readonly IFakeResponseProvider fakeResponseProvider;

        public FakeHttpClientWrapper(IFakeResponseProvider fakeResponseProvider)
        {
            this.fakeResponseProvider = fakeResponseProvider;
        }

        public Task<HttpResponseMessage> SendAsync(HttpClient client, HttpRequestMessage request, CancellationToken cancellation)
        {
            var rsp = fakeResponseProvider.TryGetFakeResponseFor(request);

            if (rsp == null) return client.SendAsync(request, cancellation);

            return Task.FromResult(rsp);
        }
    }
}
