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
    }

    public record TestDtoForSerialization
    {
        public string Name { get; init; }
    }
}
