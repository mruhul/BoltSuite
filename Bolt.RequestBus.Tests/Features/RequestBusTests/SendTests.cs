using System.Collections.Generic;
using System.Threading.Tasks;
using Bolt.RequestBus.Tests.Infra;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Bolt.RequestBus.Tests.Features.RequestBusTests
{
    public class SendTests
    {
        [Fact]
        public void Should_Run_First_Applicable_Handler()
        {
            var sut = IocHelper.GetRequestBus(sc =>
            {
                sc.AddTransient<IRequestHandler<TestRequest, None>, TestNoApplicableRequestHandler>();
                sc.AddTransient<IRequestHandler<TestRequest, None>, TestRequestHandler>();
            });

            var request = new TestRequest();

            sut.Send(request);

            request.HandlersExecuted.ShouldContain(nameof(TestRequestHandler));
        }

        [Fact]
        public void Should_Return_Response_From_First_Applicable_Handler()
        {
            var sut = IocHelper.GetRequestBus(sc =>
            {
                sc.AddTransient<IRequestHandler<TestRequest, None>, TestNoApplicableRequestHandler>();
                sc.AddTransient<IRequestHandler<TestRequest, None>, TestRequestHandler>();
            });

            var request = new TestRequest();

            sut.Send(request);

            request.HandlersExecuted.ShouldContain(nameof(TestRequestHandler));
        }

        [Fact]
        public void Should_Throw_Exception_When_No_Handler_Available()
        {
            var sut = IocHelper.GetRequestBus();

            Should.Throw<NoHandlerAvailable>(() => sut.Send<TestRequest>(new TestRequest()));
        }

        [Fact]
        public void Should_Make_RequestBus_Context_Available_To_Handler()
        {
            var sut = IocHelper.GetRequestBus(sc =>
            {
                sc.AddTransient<IRequestBusContextWriter, RequestContextWriter>();
                sc.AddTransient<IRequestHandler<TestRequestContext, None>, TestRequestContextHandler>();
            });

            var request = new TestRequestContext();
            sut.Send(request);
            request.MsgInContext.ShouldBe("Hello World!");
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

        class TestRequestContextHandler : RequestHandler<TestRequestContext>
        {
            protected override Response Handle(IRequestBusContext context, TestRequestContext request)
            {
                request.MsgInContext = context.GetOrDefault<string>("message");
                return true;
            }
        }

        class TestRequest
        {
            public List<string> HandlersExecuted { get; set; } = new List<string>();
        }

        class TestRequestHandler : RequestHandler<TestRequest>
        {
            protected override Response Handle(IRequestBusContext context, TestRequest request)
            {
                request.HandlersExecuted.Add(this.GetType().Name);
                return true;
            }
        }

        class TestNoApplicableRequestHandler : RequestHandler<TestRequest>
        {
            public override bool IsApplicable(IRequestBusContext context, TestRequest request) => false;
            protected override Response Handle(IRequestBusContext context, TestRequest request)
            {
                request.HandlersExecuted.Add(this.GetType().Name);
                return true;
            }
        }
    }
}