using System;
using System.Collections.Concurrent;
using System.Net.Http;
using System.Threading.Tasks;

namespace Bolt.FluentHttpClient.Fakes
{
    public interface IFakeResponseProvider
    {
        void AddFakeResponse(Func<HttpRequestMessage, HttpResponseMessage> fakes);
        Task<HttpResponseMessage> TryGetFakeResponseFor(HttpRequestMessage request);
    }

    internal sealed class FakeHttpResponseProvider : IFakeResponseProvider
    {

        private ConcurrentBag<Func<HttpRequestMessage, HttpResponseMessage>> fakes
            = new ConcurrentBag<Func<HttpRequestMessage, HttpResponseMessage>>();

        public void AddFakeResponse(Func<HttpRequestMessage, HttpResponseMessage> fake)
        {
            fakes.Add(fake);
        }

        public Task<HttpResponseMessage> TryGetFakeResponseFor(HttpRequestMessage request)
        {
            foreach(var fake in fakes)
            {
                var msg = fake(request);
                if (msg != null)
                {
                    msg.Headers.Add("x-fake-response", "true");

                    return Task.FromResult(msg);
                }
            }

            return Task.FromResult<HttpResponseMessage>(null);
        }
    }
}
