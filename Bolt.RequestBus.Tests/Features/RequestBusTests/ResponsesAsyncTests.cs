using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bolt.RequestBus.Tests.Infra;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Bolt.RequestBus.Tests.Features.RequestBusTests
{
    public class ResponsesAsyncTests
    {
        [Fact]
        public async Task Should_Return_Collection_Of_Responses_From_All_Applicable_Handlers()
        {
            var sut = IocHelper.GetRequestBus(sc =>
            {
                sc.AddTransient<IResponseHandlerAsync<TestRequest, TestResult>, DependentResponseHandler>();
                sc.AddTransient<IResponseHandlerAsync<TestRequest, TestResult>, IndependentResponseHandler>();
                sc.AddTransient<IResponseHandlerAsync<TestRequest, TestResult>, MainResponseHandler>();
                sc.AddTransient<IResponseHandlerAsync<TestRequest, TestResult>, DependentNonApplicableResponseHandler>();
            });
            
            var request = new TestRequest
            {
                Name = "Ruhul"
            };
            
            var rsp = await sut.ResponsesAsync<TestRequest, TestResult>(request);
            
            rsp.MainResponse.ShouldNotBeNull();
            rsp.OtherResponses.Count().ShouldBe(2);
            rsp.MainResponse.IsSucceed.ShouldBe(true);
            rsp.OtherResponses.ShouldAllBe(x => x.IsSucceed);
            rsp.OtherResponses.ShouldAllBe(x => x.Value != null);
            rsp.MainResponse.Value.Message.ShouldBe($"{request.Name} : handled by {nameof(MainResponseHandler)}");
            rsp.OtherResponses.ShouldContain(x => 
                x.Value.Message == $"{request.Name} : handled by {nameof(DependentResponseHandler)}");
            rsp.OtherResponses.ShouldContain(x => 
                x.Value.Message == $"{request.Name} : handled by {nameof(IndependentResponseHandler)}");
            rsp.OtherResponses.ShouldNotContain(x => 
                x.Value.Message == $"{request.Name} : handled by {nameof(DependentNonApplicableResponseHandler)}");
            
        }

        [Fact]
        public async Task Should_Not_Run_Other_Dependent_Handlers_When_Main_Handler_Failed()
        {
            var sut = IocHelper.GetRequestBus(sc =>
            {
                sc.AddTransient<IResponseHandlerAsync<TestRequest, TestResult>, DependentResponseHandler>();
                sc.AddTransient<IResponseHandlerAsync<TestRequest, TestResult>, IndependentResponseHandler>();
                sc.AddTransient<IResponseHandlerAsync<TestRequest, TestResult>, MainFailedResponseHandler>();
                sc.AddTransient<IResponseHandlerAsync<TestRequest, TestResult>, DependentNonApplicableResponseHandler>();
            });
            
            var request = new TestRequest
            {
                Name = "Ruhul"
            };
            
            var rsp = await sut.ResponsesAsync<TestRequest, TestResult>(request);
            
            rsp.MainResponse.ShouldNotBeNull();
            rsp.MainResponse.IsSucceed.ShouldBe(false);
            rsp.MainResponse.Errors.Count().ShouldBe(0);
            rsp.OtherResponses.Count().ShouldBe(1);
        }

        [Fact]
        public async Task Exception_In_NonMain_Handler_Should_Not_Stop_Other_Handlers_Execution()
        {
            var sut = IocHelper.GetRequestBus(sc =>
            {
                sc.AddTransient<IResponseHandlerAsync<TestRequest, TestResult>, InDependentHandlerWithException>();
                sc.AddTransient<IResponseHandlerAsync<TestRequest, TestResult>, DependentHandlerWithException>();
                sc.AddTransient<IResponseHandlerAsync<TestRequest, TestResult>, MainResponseHandler>();
            });
            
            var request = new TestRequest
            {
                Name = "Ruhul"
            };
            
            var rsp = await sut.ResponsesAsync<TestRequest, TestResult>(request);
            
            rsp.MainResponse.IsSucceed.ShouldBe(true);
            rsp.MainResponse.Value.Message.ShouldBe($"{request.Name} : handled by {nameof(MainResponseHandler)}");
            rsp.MainResponse.Errors.Count().ShouldBe(0);
            rsp.OtherResponses.Count().ShouldBe(0);
        }

        [Fact]
        public async Task Filters_Should_Change_Final_Response_After_All_Handlers_Executed()
        {
            var sut = IocHelper.GetRequestBus(sc =>
            {
                sc.AddTransient<IResponseHandlerAsync<TestRequest, TestResult>, MainResponseHandler>();
                sc.AddTransient<IResponseHandlerAsync<TestRequest, TestResult>, DependentResponseHandler>();
                sc.AddTransient<IResponseFilterAsync<TestRequest, TestResult>, ResponseFilter>();
                sc.AddTransient<IResponseFilterAsync<TestRequest, TestResult>, ResponseFilterNotApplicable>();
            });
            
            var request = new TestRequest
            {
                Name = "Ruhul"
            };

            var rsp = await sut.ResponsesAsync<TestRequest, TestResult>(request);
            
            rsp.MainResponse.ShouldNotBeNull();
            rsp.MainResponse.IsSucceed.ShouldBe(true);
            rsp.OtherResponses.Count().ShouldBe(2);
            rsp.OtherResponses.ShouldContain(x => x.Value.Message ==  $"{request.Name} filtered by {nameof(ResponseFilter)}");
        }

        [Fact]
        public async Task Should_Validate_Request()
        {
            var sut = IocHelper.GetRequestBus(sc =>
            {
                sc.AddTransient<IRequestValidatorAsync<TestRequest>, TestRequestValidator>();
            });

            var rsp =  await sut.ResponsesAsync<TestRequest, TestResult>(new TestRequest());
            
            rsp.MainResponse.ShouldNotBeNull();
            rsp.MainResponse.IsSucceed.ShouldBe(false);
            rsp.MainResponse.Errors.ShouldContain(x => x.Message == "Name is required.");
        }

        public class ResponseFilter : ResponseFilterAsync<TestRequest, TestResult>
        {
            protected override Task Filter(IRequestBusContext context, TestRequest request, IResponseCollection<TestResult> rspCollection)
            {
                rspCollection.AddResponse(Response.Succeed(new TestResult
                {
                    Message = $"{request.Name} filtered by {this.GetType().Name}"
                }));
                
                return Task.CompletedTask;
            }
        }
        
        public class ResponseFilterNotApplicable : ResponseFilterAsync<TestRequest, TestResult>
        {
            protected override Task Filter(IRequestBusContext context, TestRequest request, IResponseCollection<TestResult> rspCollection)
            {
                rspCollection.AddResponse(Response.Succeed(new TestResult
                {
                    Message = $"{request.Name} filtered by {this.GetType().Name}"
                }));
                
                return Task.CompletedTask;
            }

            public override bool IsApplicable(IRequestBusContext context, TestRequest request) => false;
        }
        
        public class TestRequestValidator : RequestValidatorAsync<TestRequest>
        {
            public override Task<IEnumerable<IError>> Validate(IRequestBusContext context, TestRequest request)
            {
                var result = new List<IError>();

                if (string.IsNullOrWhiteSpace(request.Name))
                {
                    result.Add(Error.Create("Name is required.", "Name"));
                }

                return Task.FromResult((IEnumerable<IError>)result);
            }
        }
        
        public class MainFailedResponseHandler : IResponseHandlerAsync<TestRequest, TestResult>
        {
            public Task<IResponse<TestResult>> Handle(IRequestBusContext context, TestRequest request)
            {
                return Task.FromResult(Response.Failed<TestResult>());
            }

            public bool IsApplicable(IRequestBusContext context, TestRequest request) => true;

            public ExecutionHint ExecutionHint { get; } = ExecutionHint.Main;
        }
        
        public class MainResponseHandler : ResponseHandlerAsync<TestRequest, TestResult>
        {
            protected override Task<TestResult> Handle(IRequestBusContext context, TestRequest request)
            {
                return Task.FromResult(new TestResult
                {
                    Message = $"{request.Name} : handled by {this.GetType().Name}"
                });
            }

            public override ExecutionHint ExecutionHint => ExecutionHint.Main;
        }
        
        public class IndependentResponseHandler : ResponseHandlerAsync<TestRequest, TestResult>
        {
            protected override Task<TestResult> Handle(IRequestBusContext context, TestRequest request)
            {
                return Task.FromResult(new TestResult
                {
                    Message = $"{request.Name} : handled by {this.GetType().Name}"
                });
            }

            public override ExecutionHint ExecutionHint => ExecutionHint.Independent;
        }
        
        public class DependentResponseHandler : ResponseHandlerAsync<TestRequest, TestResult>
        {
            protected override Task<TestResult> Handle(IRequestBusContext context, TestRequest request)
            {
                return Task.FromResult(new TestResult
                {
                    Message = $"{request.Name} : handled by {this.GetType().Name}"
                });
            }
        }
        
        class DependentNonApplicableResponseHandler : ResponseHandlerAsync<TestRequest, TestResult>
        {
            protected override Task<TestResult> Handle(IRequestBusContext context, TestRequest request)
            {
                return Task.FromResult( new TestResult
                {
                    Message = $"{request.Name} : handled by {this.GetType().Name}"
                });
            }

            public override bool IsApplicable(IRequestBusContext context, TestRequest request) => false;
        }
        
        public class DependentHandlerWithException : ResponseHandlerAsync<TestRequest,TestResult>
        {
            protected override Task<TestResult> Handle(IRequestBusContext context, TestRequest request)
            {
                throw new System.NotImplementedException();
            }
        }
        
        public class InDependentHandlerWithException : ResponseHandlerAsync<TestRequest,TestResult>
        {
            protected override Task<TestResult> Handle(IRequestBusContext context, TestRequest request)
            {
                throw new System.NotImplementedException();
            }

            public override ExecutionHint ExecutionHint => ExecutionHint.Independent;
        }
        
        public class TestResult
        {
            public string Message { get; set; }
        }
        
        public class TestRequest
        {
            public string Name { get; set; }
        }
    }
}