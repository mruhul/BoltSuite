using Xunit;
using Shouldly;

namespace Bolt.PubSub.RabbitMq.Tests
{
    public class Serializer_Tests : TestWithIoc
    {
        private readonly IMessageSerializer serializer;

        public Serializer_Tests(IocFixture fixture) : base(fixture)
        {
            serializer = GetService<IMessageSerializer>();
        }

        [Fact]
        public void Should_serialize_and_then_deserialize()
        {
            var dto = new TestDtoForSerialization
            {
                Name = "test-name"
            };

            var serializedData = serializer.Serialize(dto);

            var got = serializer.Deserialize<TestDtoForSerialization>(serializedData);

            dto.ShouldBe(got);
        }

        [Fact]
        public void Should_serialize_date_in_utc_format()
        {
            var dateTime = System.DateTime.Now;

            var dto = new DateTimeSerializationTest
            {
                LocalTime = dateTime,
                UtcTime = dateTime.ToUniversalTime()
            };

            var serializedBytes = serializer.Serialize(dto);

            var data = serializer.Deserialize<DateTimeSerializationTest>(serializedBytes);

            data.LocalTime.ToUniversalTime().ShouldBe(data.UtcTime);
        }
    }

    public record TestDtoForSerialization
    {
        public string Name { get; init; }
    }

    public record DateTimeSerializationTest
    {
        public System.DateTime LocalTime { get; init; }
        public System.DateTime UtcTime { get; init; }
    }
}
