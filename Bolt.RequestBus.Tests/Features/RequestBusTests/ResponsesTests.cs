using System.Collections.Generic;
using System.Linq;
using Bolt.RequestBus.Tests.Infra;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Bolt.RequestBus.Tests.Features.RequestBusTests
{
    public class ResponsesTests
    {
        [Fact]
        public void Should_Return_Collection_Of_Responses_From_All_Applicable_Handlers()
        {
            var sut = IocHelper.GetRequestBus(sc =>
            {
                sc.AddTransient<IResponseHandler<TestRequest, TestResult>, DependentResponseHandler>();
                sc.AddTransient<IResponseHandler<TestRequest, TestResult>, IndependentResponseHandler>();
                sc.AddTransient<IResponseHandler<TestRequest, TestResult>, MainResponseHandler>();
                sc.AddTransient<IResponseHandler<TestRequest, TestResult>, DependentNonApplicableResponseHandler>();
            });
            
            var request = new TestRequest
            {
                Name = "Ruhul"
            };
            
            var rsp = sut.Responses<TestRequest, TestResult>(request);
            
            rsp.Count().ShouldBe(3);
            rsp.ShouldAllBe(x => x.IsSucceed);
            rsp.ShouldAllBe(x => x.Value != null);
            rsp.ShouldContain(x => x.Value.Message == $"{request.Name} : handled by {nameof(DependentResponseHandler)}");
            rsp.ShouldContain(x => x.Value.Message == $"{request.Name} : handled by {nameof(MainResponseHandler)}");
            rsp.ShouldContain(x => x.Value.Message == $"{request.Name} : handled by {nameof(IndependentResponseHandler)}");
            rsp.ShouldNotContain(x => x.Value.Message == $"{request.Name} : handled by {nameof(DependentNonApplicableResponseHandler)}");
            
        }

        [Fact]
        public void Should_Not_Run_Other_Handlers_When_Main_Handler_Failed()
        {
            var sut = IocHelper.GetRequestBus(sc =>
            {
                sc.AddTransient<IResponseHandler<TestRequest, TestResult>, DependentResponseHandler>();
                sc.AddTransient<IResponseHandler<TestRequest, TestResult>, IndependentResponseHandler>();
                sc.AddTransient<IResponseHandler<TestRequest, TestResult>, MainFailedResponseHandler>();
                sc.AddTransient<IResponseHandler<TestRequest, TestResult>, DependentNonApplicableResponseHandler>();
            });
            
            var request = new TestRequest
            {
                Name = "Ruhul"
            };
            
            var rsp = sut.Responses<TestRequest, TestResult>(request);
            
            rsp.Count().ShouldBe(1);
            rsp.First().IsSucceed.ShouldBe(false);
            rsp.First().Errors.Count().ShouldBe(0);
        }

        [Fact]
        public void Exception_In_NonMain_Handler_Should_Not_Stop_Other_Handlers_Execution()
        {
            var sut = IocHelper.GetRequestBus(sc =>
            {
                sc.AddTransient<IResponseHandler<TestRequest, TestResult>, InDependentHandlerWithException>();
                sc.AddTransient<IResponseHandler<TestRequest, TestResult>, DependentHandlerWithException>();
                sc.AddTransient<IResponseHandler<TestRequest, TestResult>, MainResponseHandler>();
            });
            
            var request = new TestRequest
            {
                Name = "Ruhul"
            };
            
            var rsp = sut.Responses<TestRequest, TestResult>(request);
            
            rsp.Count().ShouldBe(1);
            rsp.First().IsSucceed.ShouldBe(true);
            rsp.First().Value.Message.ShouldBe($"{request.Name} : handled by {nameof(MainResponseHandler)}");
            rsp.First().Errors.Count().ShouldBe(0);
        }

        [Fact]
        public void Filters_Should_Change_Final_Response_After_All_Handlers_Executed()
        {
            var sut = IocHelper.GetRequestBus(sc =>
            {
                sc.AddTransient<IResponseHandler<TestRequest, TestResult>, MainResponseHandler>();
                sc.AddTransient<IResponseHandler<TestRequest, TestResult>, DependentResponseHandler>();
                sc.AddTransient<IResponseFilter<TestRequest, TestResult>, ResponseFilter>();
                sc.AddTransient<IResponseFilter<TestRequest, TestResult>, ResponseFilterNotApplicable>();
            });
            
            var request = new TestRequest
            {
                Name = "Ruhul"
            };

            var rsp = sut.Responses<TestRequest, TestResult>(request);
            
            rsp.Count().ShouldBe(3);
            rsp.ShouldContain(x => x.Value.Message ==  $"{request.Name} filtered by {nameof(ResponseFilter)}");
        }

        [Fact]
        public void Should_Validate_Request()
        {
            var sut = IocHelper.GetRequestBus(sc =>
            {
                sc.AddTransient<IRequestValidator<TestRequest>, TestRequestValidator>();
            });

            var rsp = sut.Responses<TestRequest, TestResult>(new TestRequest());

            rsp.Count().ShouldBe(1);
            rsp.First().IsSucceed.ShouldBe(false);
            rsp.First().Errors.ShouldContain(x => x.Message == "Name is required.");
        }

        public class ResponseFilter : ResponseFilter<TestRequest, TestResult>
        {
            protected override void Filter(IRequestBusContext context, TestRequest request, ICollection<IResponse<TestResult>> currentResult)
            {
                currentResult.Add(Response.Succeed(new TestResult
                {
                    Message = $"{request.Name} filtered by {this.GetType().Name}"
                }));
            }
        }
        
        public class ResponseFilterNotApplicable : ResponseFilter<TestRequest, TestResult>
        {
            protected override void Filter(IRequestBusContext context, TestRequest request, ICollection<IResponse<TestResult>> currentResult)
            {
                currentResult.Add(Response.Succeed(new TestResult
                {
                    Message = $"{request.Name} filtered by {this.GetType().Name}"
                }));
            }

            public override bool IsApplicable(IRequestBusContext context, TestRequest request) => false;
        }
        
        public class TestRequestValidator : RequestValidator<TestRequest>
        {
            public override IEnumerable<IError> Validate(IRequestBusContext context, TestRequest request)
            {
                if (string.IsNullOrWhiteSpace(request.Name)) yield return Error.Create("Name is required.", "Name");
            }
        }
        
        public class MainFailedResponseHandler : IResponseHandler<TestRequest, TestResult>
        {
            public IResponse<TestResult> Handle(IRequestBusContext context, TestRequest request)
            {
                return Response.Failed<TestResult>();
            }

            public bool IsApplicable(IRequestBusContext context, TestRequest request) => true;

            public ExecutionHint ExecutionHint { get; } = ExecutionHint.Main;
        }
        
        public class MainResponseHandler : ResponseHandler<TestRequest, TestResult>
        {
            protected override TestResult Handle(IRequestBusContext context, TestRequest request)
            {
                return new TestResult
                {
                    Message = $"{request.Name} : handled by {this.GetType().Name}"
                };
            }

            public override ExecutionHint ExecutionHint => ExecutionHint.Main;
        }
        
        public class IndependentResponseHandler : ResponseHandler<TestRequest, TestResult>
        {
            protected override TestResult Handle(IRequestBusContext context, TestRequest request)
            {
                return new TestResult
                {
                    Message = $"{request.Name} : handled by {this.GetType().Name}"
                };
            }

            public override ExecutionHint ExecutionHint => ExecutionHint.Independent;
        }
        
        public class DependentResponseHandler : ResponseHandler<TestRequest, TestResult>
        {
            protected override TestResult Handle(IRequestBusContext context, TestRequest request)
            {
                return new TestResult
                {
                    Message = $"{request.Name} : handled by {this.GetType().Name}"
                };
            }
        }
        
        class DependentNonApplicableResponseHandler : ResponseHandler<TestRequest, TestResult>
        {
            protected override TestResult Handle(IRequestBusContext context, TestRequest request)
            {
                return new TestResult
                {
                    Message = $"{request.Name} : handled by {this.GetType().Name}"
                };
            }

            public override bool IsApplicable(IRequestBusContext context, TestRequest request) => false;
        }
        
        public class DependentHandlerWithException : ResponseHandler<TestRequest,TestResult>
        {
            protected override TestResult Handle(IRequestBusContext context, TestRequest request)
            {
                throw new System.NotImplementedException();
            }
        }
        
        public class InDependentHandlerWithException : ResponseHandler<TestRequest,TestResult>
        {
            protected override TestResult Handle(IRequestBusContext context, TestRequest request)
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