# Bolt.FluentHttpClient

Add fluent interface to easily send http request and read response

## How to add this lib in project

Add nuget package `Bolt.FluentHttpClient` in your .net core project. Register this lib in your DI container.

```
  services.AddFluentHttpClient()
          .AddDefaultHttpHandlers();
```

## How to use this lib

You need to inject `IFluentHttpClient` in your class as below.

```
  public class BookApiProxy : IBookApiProxy
  {
    private IFluentHttpClient http;

    public BookApiProxy(IFluentHttpClient http)
    {
      this.http = http;
    }

    public async Task<Book> GetBookById(string id)
    {
      var rsp = await http
                    .ForUrl($"http://books-api.com/v1/books/{id}")
                    .QueryString("schema","simple")
                    .Header("name","value")
                    .Timeout(Timespan.FromSeconds(5))
                    .Retry(1) // retrun 1 time on failure like InternalServerError/RequestTimeout
                    //.OnFailure((statsuCode, stream, cancelletionToken) => ...) // if you like to read content of failure scenario from stream
                    //.OnFailureFromString((statsuCode, str, cancelletionToken) => ...) // if you like to read content of failure scenario from string content
                    .GetAsync<Book>();

      // rsp.Headers provide your dictionary of headers
      // rsp.StatusCode return httpstatuscode of the response
      // rsp.IsSuccessStatusCode provide whenre response has successfull status code or not.
      return rsp.Content;
    }
  }
```
