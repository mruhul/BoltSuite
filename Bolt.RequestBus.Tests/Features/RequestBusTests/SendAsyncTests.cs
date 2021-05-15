using System.Collections.Generic;
using System.Threading.Tasks;
using Bolt.RequestBus.Tests.Infra;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Bolt.RequestBus.Tests.Features.RequestBusTests
{
    public class SendAsyncTests
    {
        [Fact]
        public async Task Should_Run_First_Applicable_Handler()
        {
            var sut = IocHelper.GetRequestBus(sc =>
            {
                sc.AddTransient<IRequestHandlerAsync<TestRequest, None>, TestNoApplicableRequestHandler>();
                sc.AddTransient<IRequestHandlerAsync<TestRequest, None>, TestRequestHandler>();
            });

            var request = new TestRequest();

            await sut.SendAsync(request);

            request.HandlersExecuted.ShouldContain(nameof(TestRequestHandler));
        }

        [Fact]
        public async Task Should_Return_Response_From_First_Applicable_Handler()
        {
            var sut = IocHelper.GetRequestBus(sc =>
            {
                sc.AddTransient<IRequestHandlerAsync<TestRequest, None>, TestNoApplicableRequestHandler>();
                sc.AddTransient<IRequestHandlerAsync<TestRequest, None>, TestRequestHandler>();
            });

            var request = new TestRequest();

            await sut.SendAsync(request);

            request.HandlersExecuted.ShouldContain(nameof(TestRequestHandler));
        }

        [Fact]
        public async Task Should_Throw_Exception_When_No_Handler_Available()
        {
            var sut = IocHelper.GetRequestBus();

            await Should.ThrowAsync<NoHandlerAvailable>(() => sut.SendAsync(new TestRequest()));
        }

        [Fact]
        public async Task Should_Make_RequestBus_Context_Available_To_Handler()
        {
            var sut = IocHelper.GetRequestBus(sc =>
            {
                sc.AddTransient<IRequestBusContextWriter, RequestContextWriter>();
                sc.AddTransient<IRequestHandlerAsync<TestRequestContext, None>, TestRequestContextHandler>();
            });

            var request = new TestRequestContext();
            await sut.SendAsync(request);
            request.MsgInContext.ShouldBe("Hello World!");
        }

        [Fact]
        public async Task Should_Validate_Request()
        {
            var sut = IocHelper.GetRequestBus(sc =>
            {
                sc.AddTransient<IRequestHandlerAsync<RequestForShouldValidateRequest, None>,
                    RequestHandlerForShouldValidateRequest>();

                sc.AddTransient<IRequestValidatorAsync<RequestForShouldValidateRequest>,
                    RequestValidatorNotApplicableForShouldValidateRequest>();

                sc.AddTransient<IRequestValidatorAsync<RequestForShouldValidateRequest>,
                    RequestValidatorForShouldValidateRequest>();
            });

            var rsp = await sut.SendAsync<RequestForShouldValidateRequest>(new RequestForShouldValidateRequest());

            rsp.IsSucceed.ShouldBe(false);
            rsp.Errors.ShouldContain(x =>
                x.Message == "Name is required."
                && x.PropertyName == "Name");
        }

        class RequestForShouldValidateRequest
        {
            public string Name { get; set; }
        }

        class RequestValidatorForShouldValidateRequest : RequestValidatorAsync<RequestForShouldValidateRequest>
        {
            public override Task<IEnumerable<Error>> Validate(IRequestBusContext context,
                RequestForShouldValidateRequest request)
            {
                var list = new List<Error>();

                if (string.IsNullOrWhiteSpace(request.Name))
                    list.Add(Error.Create("Name is required.", nameof(request.Name)));

                return Task.FromResult((IEnumerable<Error>) list);
            }
        }

        class RequestValidatorNotApplicableForShouldValidateRequest : RequestValidatorAsync<
            RequestForShouldValidateRequest>
        {
            public override Task<IEnumerable<Error>> Validate(IRequestBusContext context,
                RequestForShouldValidateRequest request)
            {
                var list = new List<Error>();

                if (string.IsNullOrWhiteSpace(request.Name))
                    list.Add(Error.Create("Name is required. Please enter name.", nameof(request.Name)));

                return Task.FromResult((IEnumerable<Error>) list);
            }

            public override bool IsApplicable(IRequestBusContext context, RequestForShouldValidateRequest request)
                => false;
        }

        class RequestHandlerForShouldValidateRequest : RequestHandlerAsync<RequestForShouldValidateRequest>
        {
            protected override async Task<Response> Handle(IRequestBusContext context, RequestForShouldValidateRequest request)
            {
                request.Name = "Surprise!";
                return Response.Ok();
            }
        }

        class RequestContextWriter : IRequestBusContextWriter
        {
            public void Write(IRequestBusContext context)
            {
                context.Set("message", "Hello World!");
            }
        }

        class TestRequestContext
        {
            public string MsgInContext { get; set; }
        }

        class TestRequestContextHandler : RequestHandlerAsync<TestRequestContext>
        {
            protected override async Task<Response> Handle(IRequestBusContext context, TestRequestContext request)
            {
                request.MsgInContext = context.GetOrDefault<string>("message");
                return Response.Ok();
            }
        }

        class TestRequest
        {
            public List<string> HandlersExecuted { get; set; } = new List<string>();
        }

        class TestRequestHandler : RequestHandlerAsync<TestRequest>
        {
            protected override async Task<Response> Handle(IRequestBusContext context, TestRequest request)
            {
                request.HandlersExecuted.Add(this.GetType().Name);
                return Response.Ok();
            }
        }

        class TestNoApplicableRequestHandler : RequestHandlerAsync<TestRequest>
        {
            protected override async Task<Response> Handle(IRequestBusContext context, TestRequest request)
            {
                request.HandlersExecuted.Add(this.GetType().Name);
                return Response.Ok();
            }

            public override bool IsApplicable(IRequestBusContext context, TestRequest request) => false;
        }
    }
}