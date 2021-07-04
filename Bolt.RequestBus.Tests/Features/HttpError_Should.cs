using System;
using System.Collections.Generic;
using System.Net;
using Shouldly;
using Xunit;

namespace Bolt.RequestBus.Tests.Features
{
    public class HttpError_Should
    {
        [Theory]
        [MemberData(nameof(TestDataToConvertResponse))]
        public void Implicitly_convert_to_response(HttpError error, Response expectedResponse)
        {
            Response gotResponse = error;

            gotResponse.ShouldSatisfyAllConditions(
                () => gotResponse.IsSucceed.ShouldBe(expectedResponse.IsSucceed),
                () => gotResponse.StatusCode.ShouldBe(expectedResponse.StatusCode),
                () => gotResponse.StatusReason.ShouldBe(expectedResponse.StatusReason),
                () => gotResponse.Errors.Length.ShouldBe(0)
            );
        }

        [Theory]
        [MemberData(nameof(TestDataToConvertResponseOfT))]
        public void Implicitly_convert_to_response_of_t(HttpError error, Response<string> expectedResponse)
        {
            Response<string> gotResponse = error;

            gotResponse.ShouldSatisfyAllConditions(
                () => gotResponse.IsSucceed.ShouldBe(expectedResponse.IsSucceed),
                () => gotResponse.StatusCode.ShouldBe(expectedResponse.StatusCode),
                () => gotResponse.StatusReason.ShouldBe(expectedResponse.StatusReason),
                () => gotResponse.Errors.Length.ShouldBe(0),
                () => gotResponse.Value.ShouldBeNull()
            );
        }

        [Fact]
        public void Throw_exception_when_status_is_successful()
        {
            var gotException = Should.Throw<Exception>(() => HttpError.New(HttpStatusCode.OK, "Should throw"));
            gotException.Message.ShouldBe("HttpError only allow non successful status code.");
        }


        public static IEnumerable<object[]> TestDataToConvertResponse => new[]
        {
            new object[]
            {
                HttpError.FailedDependency("Testing dependency failed"),
                new Response
                {
                    IsSucceed = false,
                    StatusCode = (int)HttpStatusCode.FailedDependency,
                    StatusReason = "Testing dependency failed"
                }
            },
            new object[]
            {
                HttpError.NotFound("Testing not found"),
                new Response
                {
                    IsSucceed = false,
                    StatusCode = (int)HttpStatusCode.NotFound,
                    StatusReason = "Testing not found"
                }
            },
            new object[]
            {
                HttpError.Forbidden("Testing forbidden"),
                new Response
                {
                    IsSucceed = false,
                    StatusCode = (int)HttpStatusCode.Forbidden,
                    StatusReason = "Testing forbidden"
                }
            },
            new object[]
            {
                HttpError.InternalServerError("Testing internal server error"),
                new Response
                {
                    IsSucceed = false,
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    StatusReason = "Testing internal server error"
                }
            },
            new object[]
            {
                HttpError.Locked("Testing locked"),
                new Response
                {
                    IsSucceed = false,
                    StatusCode = (int)HttpStatusCode.Locked,
                    StatusReason = "Testing locked"
                }
            },
            new object[]
            {
                HttpError.PaymentRequired("Testing payment required"),
                new Response
                {
                    IsSucceed = false,
                    StatusCode = (int)HttpStatusCode.PaymentRequired,
                    StatusReason = "Testing payment required"
                }
            },
            new object[]
            {
                HttpError.Unauthorized("Testing unauthorized"),
                new Response
                {
                    IsSucceed = false,
                    StatusCode = (int)HttpStatusCode.Unauthorized,
                    StatusReason = "Testing unauthorized"
                }
            },
            new object[]
            {
                HttpError.New(HttpStatusCode.BadGateway, "Testing new with custom status code"),
                new Response
                {
                    IsSucceed = false,
                    StatusCode = (int)HttpStatusCode.BadGateway,
                    StatusReason = "Testing new with custom status code"
                }
            }
        };

        public static IEnumerable<object[]> TestDataToConvertResponseOfT => new[]
        {
            new object[]
            {
                HttpError.FailedDependency("Testing dependency failed"),
                new Response<string>
                {
                    IsSucceed = false,
                    StatusCode = (int)HttpStatusCode.FailedDependency,
                    StatusReason = "Testing dependency failed"
                }
            },
            new object[]
            {
                HttpError.NotFound("Testing not found"),
                new Response<string>
                {
                    IsSucceed = false,
                    StatusCode = (int)HttpStatusCode.NotFound,
                    StatusReason = "Testing not found"
                }
            },
            new object[]
            {
                HttpError.Forbidden("Testing forbidden"),
                new Response<string>
                {
                    IsSucceed = false,
                    StatusCode = (int)HttpStatusCode.Forbidden,
                    StatusReason = "Testing forbidden"
                }
            },
            new object[]
            {
                HttpError.InternalServerError("Testing internal server error"),
                new Response<string>
                {
                    IsSucceed = false,
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    StatusReason = "Testing internal server error"
                }
            },
            new object[]
            {
                HttpError.Locked("Testing locked"),
                new Response<string>
                {
                    IsSucceed = false,
                    StatusCode = (int)HttpStatusCode.Locked,
                    StatusReason = "Testing locked"
                }
            },
            new object[]
            {
                HttpError.PaymentRequired("Testing payment required"),
                new Response<string>
                {
                    IsSucceed = false,
                    StatusCode = (int)HttpStatusCode.PaymentRequired,
                    StatusReason = "Testing payment required"
                }
            },
            new object[]
            {
                HttpError.Unauthorized("Testing unauthorized"),
                new Response<string>
                {
                    IsSucceed = false,
                    StatusCode = (int)HttpStatusCode.Unauthorized,
                    StatusReason = "Testing unauthorized"
                }
            },
            new object[]
            {
                HttpError.New(HttpStatusCode.BadGateway, "Testing new with custom status code"),
                new Response<string>
                {
                    IsSucceed = false,
                    StatusCode = (int)HttpStatusCode.BadGateway,
                    StatusReason = "Testing new with custom status code"
                }
            }
        };
    }
}
