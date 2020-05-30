using System.Threading.Tasks;
using Bolt.RequestBus.Tests.Infra;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Bolt.RequestBus.Tests.Features.RequestBusTests
{
    public class ResponseAsyncTests
    {
        [Fact]
        public async Task Should_Execute_Applicable_Handler()
        {
            var sut = IocHelper.GetRequestBus(sc =>
            {
                sc.AddTransient<IResponseHandlerAsync<None,TestResult>, ResponseHandler>();
                sc.AddTransient<IResponseHandlerAsync<None,TestResult>, ResponseHandlerNotApplicable>();
            });

            var rsp = await sut.ResponseAsync<TestResult>();
            
            rsp.IsSucceed.ShouldBe(true);
            rsp.Value.ShouldNotBeNull();
            rsp.Value.Message.ShouldBe("Hello World!");
        }
        
        [Fact]
        public async Task Should_Throw_Exception_When_No_Handler_Available()
        {
            var sut = IocHelper.GetRequestBus(sc =>
            {
                sc.AddTransient<IResponseHandlerAsync<None,TestResult>, ResponseHandlerNotApplicable>();
            });

            await Should.ThrowAsync<NoHandlerAvailable>(() => sut.ResponseAsync<TestResult>());
        }

        class ResponseHandler : ResponseHandlerAsync<TestResult>
        {
            protected override Task<TestResult> Handle(IRequestBusContext context)
            {
                return Task.FromResult(new TestResult
                {
                    Message = "Hello World!"
                });
            }
        }
        
        class ResponseHandlerNotApplicable : ResponseHandlerAsync<TestResult>
        {
            protected override Task<TestResult> Handle(IRequestBusContext context)
            {
                return Task.FromResult(new TestResult
                {
                    Message = "Shouldn't call"
                });
            }

            protected override bool IsApplicable(IRequestBusContext context) => false;
        }
        
        class TestResult
        {
            public string Message { get; set; }
        }
    }
}