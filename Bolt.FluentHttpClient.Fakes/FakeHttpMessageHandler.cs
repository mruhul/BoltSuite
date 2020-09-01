using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Bolt.FluentHttpClient.Fakes
{
    public class FakeHttpMessageHandler : DelegatingHandler
    {
        private readonly IFakeResponseProvider fakeResponseProvider;

        public FakeHttpMessageHandler(IFakeResponseProvider fakeResponseProvider)
        {
            this.fakeResponseProvider = fakeResponseProvider;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var rsp = await fakeResponseProvider.TryGetFakeResponseFor(request);

            if (rsp == null) return await base.SendAsync(request, cancellationToken);

            return rsp;
        }
    }
}
