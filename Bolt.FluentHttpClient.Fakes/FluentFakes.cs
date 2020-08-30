using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Bolt.FluentHttpClient.Fakes
{
    public interface IFakeResponseCollectCondition
    {
        IFakeResponseHaveCondition RequestMatch(Func<HttpRequestMessage, bool> condition);
    }

    public interface IFakeResponseHaveCondition : IFakeResponseCollectCondition
    {
        IFakeResponseProvider RespondWith(HttpResponseMessage msg);
    }

    public static class FakeResponseHaveConditionExtensions
    {
        public static IFluentHaveFakeResponseProvider When(this IFakeResponseProvider source)
        {
            return new FluentFakeResponseProvider(source);
        }

        public static IFakeResponseHaveCondition WhenUri(this IFakeResponseHaveCondition source, Func<Uri, bool> condition)
        {
            return source.RequestMatch((rq) => condition(rq.RequestUri));
        }

        public static IFakeResponseHaveCondition WhenHeader(this IFakeResponseHaveCondition source, Func<HttpRequestHeaders, bool> condition)
        {
            return source.RequestMatch((rq) => condition(rq.Headers));
        }

        public static IFakeResponseHaveCondition WhenHeaderContains(this IFakeResponseHaveCondition source, string name, string value)
        {
            return source.WhenHeader(x => x.Any(h => h.Key == name && string.Join(",", h.Value) == value));
        }

        public static IFakeResponseHaveCondition WhenHeaderContains(this IFakeResponseHaveCondition source, string name)
        {
            return source.WhenHeader(x => x.Any(h => h.Key == name));
        }

        public static IFakeResponseProvider RespondWith<T>(this IFakeResponseHaveCondition source, HttpStatusCode statusCode, T content, params NameValueUnit[] headers)
        {
            var msg = new HttpResponseMessage
            {
                StatusCode = statusCode
            };

            if(content != null)
            {
                msg.Content = new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json");
            }

            if(headers != null)
            {
                foreach(var header in headers)
                {
                    msg.Headers.Add(header.Name, header.Value);
                }
            }

            return source.RespondWith(msg);
        }

        public static IFakeResponseProvider RespondWith(this IFakeResponseHaveCondition source, HttpStatusCode statusCode, params NameValueUnit[] headers)
        {
            return source.RespondWith<object>(statusCode, null, headers);
        }

        public static IFakeResponseProvider RespondOk<T>(this IFakeResponseHaveCondition source, T content, params NameValueUnit[] headers)
        {
            return source.RespondWith<object>(HttpStatusCode.OK, content, headers);
        }

        public static IFakeResponseProvider RespondOk<T>(this IFakeResponseHaveCondition source, params NameValueUnit[] headers)
        {
            return source.RespondOk<object>(null, headers);
        }
    }

    public interface IFluentHaveFakeResponseProvider : IFakeResponseHaveCondition
    {

    }

    internal sealed class FluentFakeResponseProvider : IFakeResponseHaveCondition, IFluentHaveFakeResponseProvider
    {
        private readonly IFakeResponseProvider fakeResponseProvider;
        private readonly List<Func<HttpRequestMessage, bool>> conditions
            = new List<Func<HttpRequestMessage, bool>>();

        public FluentFakeResponseProvider(IFakeResponseProvider fakeResponseProvider)
        {
            this.fakeResponseProvider = fakeResponseProvider;
        }

        public IFakeResponseProvider RespondWith(HttpResponseMessage msg)
        {
            fakeResponseProvider.AddFakeResponse((rq) =>
            {
                foreach(var condition in conditions)
                {
                    if (!condition(rq)) return null;
                }

                return msg;
            });

            return fakeResponseProvider;
        }

        public IFakeResponseHaveCondition RequestMatch(Func<HttpRequestMessage, bool> condition)
        {
            conditions.Add(condition);
            return this;
        }
    }
}
