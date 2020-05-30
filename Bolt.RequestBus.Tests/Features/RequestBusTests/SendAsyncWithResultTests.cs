using System.Collections.Generic;
using System.Threading.Tasks;
using Bolt.RequestBus.Tests.Infra;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Bolt.RequestBus.Tests.Features.RequestBusTests
{
    public class SendAsyncWithResultTests
    {
        [Fact]
        public async Task Should_Return_Result_By_First_Applicable_Handler()
        {
            var sut = IocHelper.GetRequestBus(sc =>
            {
                sc.AddTransient<IRequestHandlerAsync<TestRequest, TestResponse>, TestRequestNotApplicableHandler>();
                sc.AddTransient<IRequestHandlerAsync<TestRequest, TestResponse>, TestRequestHandler>();
            });

            var rsp = await sut.SendAsync<TestRequest, TestResponse>(new TestRequest());
            rsp.IsSucceed.ShouldBe(true);
            rsp.Value.HandlerExecuted.ShouldBe(nameof(TestRequestHandler));
        }

        [Fact]
        public async Task Should_Throw_Exception_When_Handler_Not_Available()
        {
            var sut = IocHelper.GetRequestBus(sc =>
            {
                sc.AddTransient<IRequestHandlerAsync<TestRequest, TestResponse>, TestRequestNotApplicableHandler
                >();
            });

            await Should.ThrowAsync<NoHandlerAvailable>(() =>
                sut.SendAsync<TestRequest, TestResponse>(new TestRequest()));
        }

        [Fact]
        public async Task Should_Populate_And_Make_RequestContext_Available_To_Handler()
        {
            var sut = IocHelper.GetRequestBus(sc =>
            {
                sc.AddTransient<IRequestHandlerAsync<TestRequest, TestResponse>, TestContextHandler>();
                sc.AddTransient<IRequestBusContextWriter, TestTenantContextWriter>();
                sc.AddTransient<IRequestBusContextWriter, TestUserContextWriter>();
            });

            var rsp = await sut.SendAsync<TestRequest, TestResponse>(new TestRequest());

            rsp.IsSucceed.ShouldBe(true);
            rsp.Value.HandlerExecuted.ShouldBe(nameof(TestContextHandler));
            rsp.Value.Message.ShouldBe("tenant:bookworm-au user:ruhul");
        }

        [Fact]
        public async Task Should_Validate_Request()
        {
            var sut = IocHelper.GetRequestBus(sc =>
            {
                sc.AddTransient<IRequestHandlerAsync<TestValidationRequest, string>, TestValidationRequestHandler>();
                sc.AddTransient<IRequestValidatorAsync<TestValidationRequest>,
                    TestNotApplicableRequestValidator>();
                sc.AddTransient<IRequestValidatorAsync<TestValidationRequest>, TestRequestValidator>();
            });

            var rsp = await sut.SendAsync<TestValidationRequest, string>(new TestValidationRequest());
            rsp.IsSucceed.ShouldBe(false);
            rsp.Value.ShouldBe(null);
            rsp.Errors.ShouldContain(err =>
                err.Message == "Name is required."
                && err.PropertyName == "Name");
        }

        class TestContextHandler : RequestHandlerAsync<TestRequest, TestResponse>
        {
            protected override Task<TestResponse> Handle(IRequestBusContext context, TestRequest request)
            {
                return Task.FromResult(new TestResponse
                {
                    HandlerExecuted = this.GetType().Name,
                    Message =
                        $"tenant:{context.GetOrDefault<string>("current-tenant")} user:{context.GetOrDefault<string>("user-name")}"
                });
            }
        }

        class TestTenantContextWriter : IRequestBusContextWriter
        {
            public void Write(IRequestBusContext context)
            {
                context.Set("current-tenant", "bookworm-au");
            }
        }

        class TestUserContextWriter : IRequestBusContextWriter
        {
            public void Write(IRequestBusContext context)
            {
                context.Set("user-name", "ruhul");
            }
        }

        class TestRequest
        {
        }

        class TestResponse
        {
            public string HandlerExecuted { get; set; }
            public string Message { get; set; }
        }

        class TestRequestHandler : RequestHandlerAsync<TestRequest, TestResponse>
        {
            protected override Task<TestResponse> Handle(IRequestBusContext context, TestRequest request)
            {
                return Task.FromResult(new TestResponse
                {
                    HandlerExecuted = this.GetType().Name
                });
            }
        }

        class TestRequestNotApplicableHandler : RequestHandlerAsync<TestRequest, TestResponse>
        {
            protected override Task<TestResponse> Handle(IRequestBusContext context, TestRequest request)
            {
                return Task.FromResult(new TestResponse
                {
                    HandlerExecuted = this.GetType().Name
                });
            }

            public override bool IsApplicable(IRequestBusContext context, TestRequest request) => false;
        }

        class TestValidationRequest
        {
            public string Name { get; set; }
        }

        class TestValidationRequestHandler : RequestHandlerAsync<TestValidationRequest, string>
        {
            protected override Task<string> Handle(IRequestBusContext context, TestValidationRequest request)
            {
                return Task.FromResult($"Hello {request.Name}!");
            }
        }

        class TestRequestValidator : RequestValidatorAsync<TestValidationRequest>
        {
            public override Task<IEnumerable<IError>> Validate(IRequestBusContext context,
                TestValidationRequest request)
            {
                var result = new List<IError>();

                if (string.IsNullOrWhiteSpace(request.Name))
                    result.Add(
                        Error.Create("Name is required.", nameof(request.Name)));

                return Task.FromResult((IEnumerable<IError>) result);
            }
        }

        class TestNotApplicableRequestValidator : RequestValidatorAsync<TestValidationRequest>
        {
            public override Task<IEnumerable<IError>> Validate(IRequestBusContext context,
                TestValidationRequest request)
            {
                var result = new List<IError>();

                if (string.IsNullOrWhiteSpace(request.Name))
                    result.Add(
                        Error.Create("Name is required. Please enter your name.", nameof(request.Name)));

                return Task.FromResult((IEnumerable<IError>) result);
            }

            public override bool IsApplicable(IRequestBusContext context, TestValidationRequest request) => false;
        }
    }
}