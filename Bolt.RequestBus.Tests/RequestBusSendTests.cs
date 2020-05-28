using System.Collections.Generic;
using Bolt.RequestBus.Tests.Infra;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Bolt.RequestBus.Tests
{
    public class RequestBusSendTests
    {
        public class SendWithResult
        {
            [Fact]
            public void Should_Return_Result_By_First_Applicable_Handler()
            {
                var sut = IocHelper.GetRequestBus(sc =>
                {
                    sc.AddTransient<IRequestHandler<TestRequest, TestResponse>,TestRequestNotApplicableHandler>();
                    sc.AddTransient<IRequestHandler<TestRequest, TestResponse>,TestRequestHandler>();
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
                
                Should.Throw<NoRequestHandlerAvailable>(() => 
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
                protected override TestResponse Handle(IRequestBusContext context, TestRequest request)
                {
                    return new TestResponse
                    {
                        HandlerExecuted = this.GetType().Name,
                        Message = $"tenant:{context.GetOrDefault<string>("current-tenant")} user:{context.GetOrDefault<string>("user-name")}"
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
                protected override TestResponse Handle(IRequestBusContext context, TestRequest request)
                {
                    return new TestResponse
                    {
                        HandlerExecuted = this.GetType().Name
                    };
                }
            }

            class TestRequestNotApplicableHandler : RequestHandler<TestRequest, TestResponse>
            {
                protected override TestResponse Handle(IRequestBusContext context, TestRequest request)
                {
                    return new TestResponse
                    {
                        HandlerExecuted = this.GetType().Name
                    };
                }

                public override bool IsApplicable(IRequestBusContext context, TestRequest request) => false;
            }
        }
        
        public class Send
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

                Should.Throw<NoRequestHandlerAvailable>(() => sut.Send<TestRequest>(new TestRequest()));
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
                protected override void Handle(IRequestBusContext context, TestRequestContext request)
                {
                    request.MsgInContext = context.GetOrDefault<string>("message");
                }
            }
            
            class TestRequest
            {
                public List<string> HandlersExecuted { get; set; } = new List<string>();
            }

            class TestRequestHandler : RequestHandler<TestRequest>
            {
                protected override void Handle(IRequestBusContext context, TestRequest request)
                {
                    request.HandlersExecuted.Add(this.GetType().Name);
                }
            }
            
            class TestNoApplicableRequestHandler : RequestHandler<TestRequest>
            {
                protected override void Handle(IRequestBusContext context, TestRequest request)
                {
                    request.HandlersExecuted.Add(this.GetType().Name);
                }

                public override bool IsApplicable(IRequestBusContext context, TestRequest request) => false;
            }
            
        }
    }
}