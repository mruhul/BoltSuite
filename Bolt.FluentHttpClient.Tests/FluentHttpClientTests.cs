using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Xunit;
using Bolt.FluentHttpClient.Fluent;
using Shouldly;
using System.Net;
using Newtonsoft.Json;
using System.Linq;

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
        public async Task PostAsync_Should_Return_BadRequest_When_Book_Title_Missing()
        {
            ErrorContainer error = null;
            var rsp = await fixture.HttpClient
                .ForUrl("http://localhost:4000/v1/books/")
                .OnFailureFromString((statusCode, sr, ct) =>
                {
                    error = JsonConvert.DeserializeObject<ErrorContainer>(sr);
                    return Task.CompletedTask;
                })
                .PostAsync(new Book
                {
                    Id = "2"
                });

            rsp.ShouldSatisfyAllConditions
            (
                () => rsp.StatusCode.ShouldBe(HttpStatusCode.BadRequest),
                () => error.ShouldNotBeNull(),
                () => error.Errors.Length.ShouldBe(1)
            );

            var err = error.Errors.FirstOrDefault();

            rsp.ShouldSatisfyAllConditions
            (
                () => err.Code.ShouldBe("TitleRequired"),
                () => err.Message.ShouldBe("Title is required."),
                () => err.PropertyName.ShouldBe("Title")
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

        [Fact]
        public async Task PutAync_Should_ReadContent()
        {
            var rsp = await fixture.HttpClient
                .ForUrl("http://localhost:4000/v1/books/3")
                .PutAsync<Book, Book>(new Book
                {
                    Title = "book3-new"
                });

            rsp.ShouldSatisfyAllConditions
            (
                () => rsp.StatusCode.ShouldBe(HttpStatusCode.OK),
                () => rsp.Content.ShouldNotBeNull(),
                () => rsp.Content.Title.ShouldBe("book3-new")
            );
        }

        [Fact]
        public async Task DeleteAsync_Should_Return_Okay()
        {
            var rsp = await fixture.HttpClient
                .ForUrl("http://localhost:4000/v1/books/4")
                .DeleteAsync();

            rsp.StatusCode.ShouldBe(HttpStatusCode.OK);
        }

        [Fact]
        public async Task DeleteAsync_Should_Return_NotFound_For_NotExistingBook()
        {
            var rsp = await fixture.HttpClient
                .ForUrl("http://localhost:4000/v1/books/100")
                .DeleteAsync();

            rsp.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        }

        internal class Book
        {
            public string Id { get; set; }
            public string Title { get; set; }
        }

        internal class ErrorContainer
        {
            public Error[] Errors { get; set; }
        }

        internal class Error
        {
            public string Code { get; set; }
            public string Message { get; set; }
            public string PropertyName { get; set; }
        }
    }
}
