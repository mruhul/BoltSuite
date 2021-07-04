using System.Linq;
using System.Net;
using Shouldly;
using Xunit;

namespace Bolt.RequestBus.Tests.Features
{
    public class Error_Should
    {
        [Fact]
        public void Implicitly_convert_to_response()
        {
            var error = Error.Create("test message", "testproperty", "testcode");
            Response gotResponse = error;
            
            gotResponse.ShouldSatisfyAllConditions
            (
                () => gotResponse.IsSucceed.ShouldBe(false),
                () => gotResponse.StatusCode.ShouldBe((int)HttpStatusCode.BadRequest),
                () => gotResponse.StatusReason.ShouldBeNull(),
                () => gotResponse.Errors.Length.ShouldBe(1),
                () => gotResponse.Errors.FirstOrDefault().ShouldBe(error)
            );
        }

        [Fact]
        public void Implicitly_convert_to_response_when_its_collection()
        {
            var errorOne = Error.Create("test message1", "testproperty1", "testcode1");
            var errorTwo = Error.Create("test message2", "testproperty2", "testcode2");

            Response gotResponse = new []{errorOne, errorTwo};

            gotResponse.ShouldSatisfyAllConditions
            (
                () => gotResponse.IsSucceed.ShouldBe(false),
                () => gotResponse.StatusCode.ShouldBe((int)HttpStatusCode.BadRequest),
                () => gotResponse.StatusReason.ShouldBeNull(),
                () => gotResponse.Errors.Length.ShouldBe(2),
                () => gotResponse.Errors[0].ShouldBe(errorOne),
                () => gotResponse.Errors[1].ShouldBe(errorTwo)
            );
        }

        [Fact]
        public void Implicitly_convert_to_response_of_t()
        {
            var error = Error.Create("test message", "testproperty", "testcode");
            Response<string> gotResponse = error;

            gotResponse.ShouldSatisfyAllConditions
            (
                () => gotResponse.IsSucceed.ShouldBe(false),
                () => gotResponse.StatusCode.ShouldBe((int)HttpStatusCode.BadRequest),
                () => gotResponse.StatusReason.ShouldBeNull(),
                () => gotResponse.Errors.Length.ShouldBe(1),
                () => gotResponse.Errors.FirstOrDefault().ShouldBe(error),
                () => gotResponse.Value.ShouldBeNull()
            );
        }



        [Fact]
        public void Implicitly_convert_to_response_of_t_when_its_collection()
        {
            var errorOne = Error.Create("test message1", "testproperty1", "testcode1");
            var errorTwo = Error.Create("test message2", "testproperty2", "testcode2");

            Response<string> gotResponse = new[] { errorOne, errorTwo };

            gotResponse.ShouldSatisfyAllConditions
            (
                () => gotResponse.IsSucceed.ShouldBe(false),
                () => gotResponse.StatusCode.ShouldBe((int)HttpStatusCode.BadRequest),
                () => gotResponse.StatusReason.ShouldBeNull(),
                () => gotResponse.Errors.Length.ShouldBe(2),
                () => gotResponse.Errors[0].ShouldBe(errorOne),
                () => gotResponse.Errors[1].ShouldBe(errorTwo),
                () => gotResponse.Value.ShouldBeNull()
            );
        }
    }
}