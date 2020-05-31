using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Bolt.RequestBus.Widgets.Tests.Infra;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Bolt.RequestBus.Widgets.Tests
{
    public class WidgetResponseTests
    {
        [Fact]
        public void Should_Return_Empty_Response_When_No_Handler_Registered()
        {
            var sut = IocHelper.GetRequestBus(sc =>
            {
            });

            var rsp = sut.WidgetResponse(new TestRequest());
            
            rsp.ShouldNotBeNull();
            rsp.StatusCode.ShouldBe((int) HttpStatusCode.OK);
            rsp.Errors.ShouldBeEmpty();
            rsp.Widgets.ShouldBeEmpty();
        }

        [Fact]
        public void Should_Return_BadRequest_With_Error_When_Validation_Failed()
        {
            var sut = IocHelper.GetRequestBus(sc =>
            {
                sc.AddTransient<IRequestValidator<TestRequest>, TestRequestValidator>();
            });

            var rsp = sut.WidgetResponse(new TestRequest());
            
            rsp.StatusCode.ShouldBe(400);
            rsp.Errors.Count().ShouldBe(1);
            rsp.Errors.ShouldContain(x => x.Message == "Name is required." && x.PropertyName == "Name");
        }

        [Fact]
        public void Should_Return_All_Widget_Responses()
        {
            
        }
        
        public class TestRequestValidator : RequestValidator<TestRequest>
        {
            public override IEnumerable<IError> Validate(IRequestBusContext context, TestRequest request)
            {
                if (string.IsNullOrWhiteSpace(request.Name))
                    yield return Error.Create("Name is required.", nameof(request.Name));
            }
        }
        
        public class TestRequest
        {
            public string Name { get; set; }
        }
    }
}