using Bolt.RequestBus.Tests.Infra;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Bolt.RequestBus.Tests.Features.RequestBusTests
{
    public class ResponseTests
    {
        [Fact]
        public void Should_Execute_Applicable_Handler()
        {
            var sut = IocHelper.GetRequestBus(sc =>
            {
                sc.AddTransient<IResponseHandler<None,TestResult>, ResponseHandler>();
                sc.AddTransient<IResponseHandler<None,TestResult>, ResponseHandlerNotApplicable>();
            });

            var rsp = sut.Response<TestResult>();
            
            rsp.IsSucceed.ShouldBe(true);
            rsp.Value.ShouldNotBeNull();
            rsp.Value.Message.ShouldBe("Hello World!");
        }
        
        [Fact]
        public void Should_Throw_Exception_When_No_Handler_Available()
        {
            var sut = IocHelper.GetRequestBus(sc =>
            {
                sc.AddTransient<IResponseHandler<None,TestResult>, ResponseHandlerNotApplicable>();
            });

            Should.Throw<NoHandlerAvailable>(() => sut.Response<TestResult>());
        }

        class ResponseHandler : ResponseHandler<TestResult>
        {
            protected override TestResult Handle(IRequestBusContext context)
            {
                return new TestResult
                {
                    Message = "Hello World!"
                };
            }
        }
        
        class ResponseHandlerNotApplicable : ResponseHandler<TestResult>
        {
            protected override TestResult Handle(IRequestBusContext context)
            {
                return new TestResult
                {
                    Message = "Shouldn't call"
                };
            }

            protected override bool IsApplicable(IRequestBusContext context) => false;
        }
        
        class TestResult
        {
            public string Message { get; set; }
        }
    }
}