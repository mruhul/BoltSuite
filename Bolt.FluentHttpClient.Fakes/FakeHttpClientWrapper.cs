﻿using System.Net.Http;
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

        public async Task<HttpResponseMessage> SendAsync(HttpClient client, HttpRequestMessage request, CancellationToken cancellation)
        {
            var rsp = await fakeResponseProvider.TryGetFakeResponseFor(request);

            if (rsp == null) return await client.SendAsync(request, cancellation);

            return rsp;
        }
    }
}
