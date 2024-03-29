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
            var sut = IocHelper.GetRequestBus(sc =>
            {
                sc.AddTransient<IResponseHandler<TestRequest, WidgetResponse>, MainTestWidget>();
                sc.AddTransient<IResponseHandler<TestRequest, WidgetResponse>, TestWidget3Rd>();
                sc.AddTransient<IResponseHandler<TestRequest, WidgetResponse>, TestWidget2Nd>();
            });

            var rsp = sut.WidgetResponse(new TestRequest());
            
            rsp.StatusCode.ShouldBe(200);
            rsp.RedirectAction.ShouldBeNull();
            rsp.Widgets.ShouldContain(x => x.Name == "main" && x.DisplayOrder == 0);
            rsp.Widgets.ShouldContain(x => x.Name == "recently-viewed" && x.DisplayOrder == 2);
            rsp.Widgets.ShouldContain(x => x.Name == "latest-editorials" && x.DisplayOrder == 1);
        }

        [Fact]
        public void Should_Return_All_Widgets_Including_Groups()
        {
            var sut = IocHelper.GetRequestBus(sc =>
            {
                sc.AddTransient<IResponseHandler<TestRequest, WidgetResponse>, MainTestWidget>();
                sc.AddTransient<IResponseHandler<TestRequest, WidgetResponse>, TestWidget3Rd>();
                sc.AddTransient<IResponseHandler<TestRequest, WidgetResponse>, TestWidget2Nd>();
                sc.AddTransient<IResponseHandler<TestRequest, WidgetResponse>, TestWidgetGroup5Th>();
                sc.AddTransient<IResponseHandler<TestRequest, WidgetResponse>, TestWidgetGroup4Th>();
            });

            var rsp = sut.WidgetResponse(new TestRequest());

            rsp.StatusCode.ShouldBe(200);
            rsp.RedirectAction.ShouldBeNull();
            rsp.Widgets.ShouldContain(x => x.Name == "main" && x.DisplayOrder == 0);
            rsp.Widgets.ShouldContain(x => x.Name == "recently-viewed" && x.DisplayOrder == 2);
            rsp.Widgets.ShouldContain(x => x.Name == "latest-editorials" && x.DisplayOrder == 1);
            rsp.Widgets.ShouldContain(x => x.Name == "docked" && x.DisplayOrder == 4);
            rsp.Widgets.First(x => x.Name == "docked").Widgets.Length.ShouldBe(2);
        }

        class MainTestWidget : WidgetResponseHandler<TestRequest>
        {
            public override Response<WidgetResponse> Handle(IRequestBusContext context, TestRequest request)
            {
                return WidgetBuilder
                    .WithName("main")
                    .WithType("intro")
                    .Build(new
                    {
                        Heading = "Welcome to app",
                        Summary = "This is summary"
                    });
            }
        }

        class TestWidget2Nd : WidgetResponseHandler<TestRequest>
        {
            public override Response<WidgetResponse> Handle(IRequestBusContext context, TestRequest request)
            {
                return WidgetBuilder
                    .WithName("recently-viewed")
                    .WithType("data-carousel")
                    .WithDisplayOrder(2)
                    .Build(new[]
                    {
                        new
                        {
                            Id = 1,
                            Heading = "This is title1",
                            PhotoUrl = "http://resource.static.com/200x160"
                        }
                    });
            }
        }
        
        class TestWidget3Rd : WidgetResponseHandler<TestRequest>
        {
            public override Response<WidgetResponse> Handle(IRequestBusContext context, TestRequest request)
            {
                return WidgetBuilder
                    .WithName("latest-editorials")
                    .WithType("data-carousel")
                    .WithDisplayOrder(1)
                    .Build(new[]
                    {
                        new
                        {
                            Id = 1,
                            Heading = "This is title1",
                            PhotoUrl = "http://resource.static.com/200x160"
                        }
                    });
            }
        }
        
        public class TestRequestValidator : RequestValidator<TestRequest>
        {
            public override IEnumerable<Error> Validate(IRequestBusContext context, TestRequest request)
            {
                if (string.IsNullOrWhiteSpace(request.Name))
                    yield return Error.Create("Name is required.", nameof(request.Name));
            }
        }
        
        public class TestRequest
        {
            public string Name { get; set; }
        }

        class TestWidgetGroup4Th : WidgetResponseHandler<TestRequest>
        {
            public override Response<WidgetResponse> Handle(IRequestBusContext context, TestRequest request)
            {
                return WidgetBuilder
                    .WithName("back")
                    .WithGroup("docked")
                    .WithType("cta")
                    .WithDisplayOrder(4)
                    .Build(new[]
                    {
                        new
                        {
                            Id = 1,
                            Heading = "This is title1",
                            PhotoUrl = "http://resource.static.com/200x160"
                        }
                    });
            }
        }

        class TestWidgetGroup5Th : WidgetResponseHandler<TestRequest>
        {
            public override Response<WidgetResponse> Handle(IRequestBusContext context, TestRequest request)
            {
                return WidgetBuilder
                    .WithName("back")
                    .WithGroup("docked")
                    .WithType("timer")
                    .WithDisplayOrder(5)
                    .Build(new[]
                    {
                        new
                        {
                            Id = 1,
                            Heading = "This is title1",
                            PhotoUrl = "http://resource.static.com/200x160"
                        }
                    });
            }
        }
    }
}