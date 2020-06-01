# Bolt.RequestBus

A library to execute requests by handlers in loosly coupled way. It helps to build easily extensible applications

## How to use this in my project?

Add nuget package Bolt.RequestBus in your project. Register the library in your IOC container as below:

    // sc is an instance of IServiceCollection
    sc.AddRequestBus();

That's it the lib is ready to use in your application.

## Example of using SendAsync method of RequestBus

Say you need to display a book details based on id. So we define a request dto as below:

    public class BookDetailsRequest
    {
        public string Id {get; set;}
    }

And you expect to return Viewmodel for details page and you define the view model as below:

    public class BookDetailsViewModel
    {
        public string Title {get; set;}
        public IEnumerable<string> Authors { get; set; }
        public string PhotoUrl { get; set; }
    }

Now we define RequestHandler for that defined request and response as below:

    public class BookDetailsRequestHandler : RequestHandlerAsync<BookDetailsRequest, BookDetailsViewModel>
    {
        private readonly IBookDetailsRepository _repo;
        private readonly AppSettings _settings;

        public BookDetailsRequestHandler(IBookDetailsRepository repo, IOptions<AppSettings> settings)
        {
            _repo = repo;
            _settings = settings.Value;
        }

        protected override async Task<BookDetailsViewModel> Handle(IRequestBusContext context, BookDetailsRequest request)
        {
            // load raw book data from db
            var detailsRow = await _repo.GetById(request.Id);

            if(detailsRow == null) return null;

            return new BookDetailsViewModel
            {
                Title = $"{detailsRow.Title} ({detailsRow.YearPublished})",
                Authors = details.Authors.Select(x => x.Name),
                PhotoUrl = $"{_settings.ImageBaseUrl}/{x.PhotoPath}"
            }
        }
    }

Make sure you register this handler in your IOC

    sc.AddTranisent<IRequestHandlerAsync<BookDetailsRequest,BookDetailsViewModel>, BookDetailsRequestHandler>();

Now here is the example how you can use this handler in your controller using `IRequestBus`

    public class BooksController : Controller
    {
        private readonly IRequestBus _bus;

        public BooksController(IRequestBus bus)
        {
            _bus = bus;
        }

        public async Task<IActionResult> Get(BookDetailsRequest request)
        {
            var vm = await _bus.SendAsync<BookDetailsRequest, BookDetailsViewModel>(request);

            if(vm == null) return NotFound();

            return Ok(vm);
        }
    }

## More documentation will come. Till now check the tests in project to get an idea all the features of this library