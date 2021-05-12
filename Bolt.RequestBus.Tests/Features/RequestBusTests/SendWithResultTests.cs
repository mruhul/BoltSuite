using Bolt.RequestBus.Tests.Infra;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Bolt.RequestBus.Tests.Features.RequestBusTests
{
    public class SendWithResultTests
    {
        [Fact]
        public void Should_Return_Result_By_First_Applicable_Handler()
        {
            var sut = IocHelper.GetRequestBus(sc =>
            {
                sc.AddTransient<IRequestHandler<TestRequest, TestResponse>, TestRequestNotApplicableHandler>();
                sc.AddTransient<IRequestHandler<TestRequest, TestResponse>, TestRequestHandler>();
            });

            var rsp = sut.Send<TestRequest, TestResponse>(new TestRequest());
            rsp.IsSucceed.ShouldBe(true);
            rsp.Value.HandlerExecuted.ShouldBe(nameof(TestRequestHandler));
        }

        [Fact]
        public void Should_Throw_Exception_When_Handler_Not_Available()
        {
            var sut = IocHelper.GetRequestBus(sc =>
            {
                sc.AddTransient<IRequestHandler<TestRequest, TestResponse>, TestRequestNotApplicableHandler>();
            });

            Should.Throw<NoHandlerAvailable>(() =>
                sut.Send<TestRequest, TestResponse>(new TestRequest()));
        }

        [Fact]
        public void Should_Populate_And_Make_RequestContext_Available_To_Handler()
        {
            var sut = IocHelper.GetRequestBus(sc =>
            {
                sc.AddTransient<IRequestHandler<TestRequest, TestResponse>, TestContextHandler>();
                sc.AddTransient<IRequestBusContextWriter, TestTenantContextWriter>();
                sc.AddTransient<IRequestBusContextWriter, TestUserContextWriter>();
            });

            var rsp = sut.Send<TestRequest, TestResponse>(new TestRequest());

            rsp.IsSucceed.ShouldBe(true);
            rsp.Value.HandlerExecuted.ShouldBe(nameof(TestContextHandler));
            rsp.Value.Message.ShouldBe("tenant:bookworm-au user:ruhul");
        }

        class TestContextHandler : RequestHandler<TestRequest, TestResponse>
        {
            public override Response<TestResponse> Handle(IRequestBusContext context, TestRequest request)
            {
                return new TestResponse
                {
                    HandlerExecuted = this.GetType().Name,
                    Message =
                        $"tenant:{context.GetOrDefault<string>("current-tenant")} user:{context.GetOrDefault<string>("user-name")}"
                };
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

        class TestRequestHandler : RequestHandler<TestRequest, TestResponse>
        {
            public override Response<TestResponse> Handle(IRequestBusContext context, TestRequest request)
            {
                return new TestResponse
                {
                    HandlerExecuted = this.GetType().Name
                };
            }
        }

        class TestRequestNotApplicableHandler : RequestHandler<TestRequest, TestResponse>
        {
            public override bool IsApplicable(IRequestBusContext context, TestRequest request) => false;
            public override Response<TestResponse> Handle(IRequestBusContext context, TestRequest request)
            {
                return new TestResponse
                {
                    HandlerExecuted = this.GetType().Name
                };
            }
        }
    }
}