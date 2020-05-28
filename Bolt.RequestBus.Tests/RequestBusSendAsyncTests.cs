using System.Collections.Generic;
using System.Threading.Tasks;
using Bolt.RequestBus.Tests.Infra;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Bolt.RequestBus.Tests
{
    public class RequestBusSendAsyncTests
    {
        public class SendAsyncWithResult
        {
            [Fact]
            public async Task Should_Return_Result_By_First_Applicable_Handler()
            {
                var sut = IocHelper.GetRequestBus(sc =>
                {
                    sc.AddTransient<IRequestHandlerAsync<TestRequest, TestResponse>,TestRequestNotApplicableHandler>();
                    sc.AddTransient<IRequestHandlerAsync<TestRequest, TestResponse>,TestRequestHandler>();
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
                    sc.AddTransient<IRequestHandlerAsync<TestRequest, TestResponse>, TestRequestNotApplicableHandler>();
                });
                
                await Should.ThrowAsync<NoRequestHandlerAvailable>(() => 
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
            
            class TestContextHandler : RequestHandlerAsync<TestRequest, TestResponse>
            {
                protected override Task<TestResponse> Handle(IRequestBusContext context, TestRequest request)
                {
                    return Task.FromResult(new TestResponse
                    {
                        HandlerExecuted = this.GetType().Name,
                        Message = $"tenant:{context.GetOrDefault<string>("current-tenant")} user:{context.GetOrDefault<string>("user-name")}"
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
        }
        
        public class SendAsync
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

                await Should.ThrowAsync<NoRequestHandlerAvailable>(() => sut.SendAsync(new TestRequest()));
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
                protected override Task Handle(IRequestBusContext context, TestRequestContext request)
                {
                    request.MsgInContext = context.GetOrDefault<string>("message");
                    return Task.CompletedTask;
                }
            }
            
            class TestRequest
            {
                public List<string> HandlersExecuted { get; set; } = new List<string>();
            }

            class TestRequestHandler : RequestHandlerAsync<TestRequest>
            {
                protected override Task Handle(IRequestBusContext context, TestRequest request)
                {
                    request.HandlersExecuted.Add(this.GetType().Name);
                    return Task.CompletedTask;
                }
            }
            
            class TestNoApplicableRequestHandler : RequestHandlerAsync<TestRequest>
            {
                protected override Task Handle(IRequestBusContext context, TestRequest request)
                {
                    request.HandlersExecuted.Add(this.GetType().Name);
                    return Task.CompletedTask;
                }

                public override bool IsApplicable(IRequestBusContext context, TestRequest request) => false;
            }
            
        }
    }
}