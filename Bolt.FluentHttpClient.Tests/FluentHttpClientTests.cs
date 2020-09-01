using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Xunit;
using Bolt.FluentHttpClient.Fluent;
using System;
using Shouldly;
using System.Net;

namespace Bolt.FluentHttpClient.Tests
{
    public class FluentHttpClientTests : IClassFixture<HttpClientFixture>
    {
        private readonly HttpClientFixture fixture;

        public FluentHttpClientTests(HttpClientFixture fixture)
        {
            this.fixture = fixture;
        }

        [Fact]
        public async Task GetAsync_Should_Return_Response()
        {
            var rsp = await fixture.HttpClient
                .ForUrl("http://localhost:4000/v1/books/1")
                .GetAsync<Book>();

            rsp.ShouldSatisfyAllConditions(
                () => rsp.IsSuccessStatusCode.ShouldBeTrue(),
                () => rsp.Content.Id.ShouldBe("1"),
                () => rsp.Content.Title.ShouldBe("book1")
            );
        }

        [Fact]
        public async Task PostAsync_Should_Create_Book()
        {
            var rsp = await fixture.HttpClient
                .ForUrl("http://localhost:4000/v1/books/")
                .PostAsync(new Book
                {
                    Id = "2",
                    Title = "book2"
                });
            
            rsp.ShouldSatisfyAllConditions
            (
                () => rsp.StatusCode.ShouldBe(HttpStatusCode.Created)
            );
        }

        [Fact]
        public async Task PostAsync_Should_Read_Content()
        {
            var rsp = await fixture.HttpClient
                .ForUrl("http://localhost:4000/v1/books/")
                .PostAsync<Book, Book>(new Book
                {
                    Title = "book2"
                });

            rsp.ShouldSatisfyAllConditions
            (
                () => rsp.StatusCode.ShouldBe(HttpStatusCode.Created),
                () => rsp.Content.ShouldNotBeNull(),
                () => rsp.Content.Title.ShouldBe("book2"),
                () => rsp.Location().StartsWith("/v1/books/").ShouldBeTrue()
            );
        }

        internal class Book
        {
            public string Id { get; set; }
            public string Title { get; set; }
        }
    }
}
