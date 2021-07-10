using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Shouldly;
using NSubstitute;
using Bolt.PubSub.RabbitMq.Publishers;
using Microsoft.Extensions.DependencyInjection;

namespace Bolt.PubSub.RabbitMq.Tests
{
    public class Publisher_Tests : TestWithIoc
    {
        public Publisher_Tests(IocFixture fixture) : base(fixture)
        {
        }

        [Fact]
        public async Task Publisher_message_can_be_alter_using_filter()
        {
            using var scope = CreateScope(sc => {
                sc.AddTransient(sc => {
                    var filter = Substitute.For<IMessageFilter>();
                    filter.Filter(Arg.Any<Message<SampleEvent>>()).Returns(c => {
                        var arg = c.Arg<Message<SampleEvent>>();
                        arg.Headers.Add("custom-header","custom-value");
                        return arg with { AppId = "new-app-id", CorrelationId = "cid" };
                    });
                    return filter;
                });
            });

            PublishMessageWrapperDto msgGot = null;
            var fakeRabbitMqPublisher = scope.ServiceProvider.GetService<Publishers.IRabbitMqPublisher>();
            fakeRabbitMqPublisher.When(x => x.Publish(Arg.Any<Publishers.PublishMessageWrapperDto>()))
                .Do(c =>
                {
                    msgGot = c.Arg<PublishMessageWrapperDto>();
                });

            var sut = scope.ServiceProvider.GetRequiredService<IMessagePublisher>();
            await sut.Publish(new SampleEvent { Name = "test" });

            msgGot.Headers[HeaderNames.AppId].ShouldBe("new-app-id");
            msgGot.CorrelationId.ShouldBe("cid");
        }


        [Theory]
        [MemberData(nameof(TestData))]
        public async Task Publisher_should_publish_message_with_correct_data(PublishTestData testData)
        {
            // Arrange
            // - Given uniqueid is provided
            var uniqueId = GetService<IUniqueId>();
            uniqueId.New().Returns(testData.GivenUniqueId);

            // - Given following settings defined for rabbitmq
            SetupSettings(testData.GivenSettings);

            // - Record the contract pass to RabbitMqPublisher
            PublishMessageWrapperDto msgGot = null;

            var serializer = GetService<IMessageSerializer>();

            SampleEvent contentGot = null;
            var fakeRabbitMqPublisher = GetService<Publishers.IRabbitMqPublisher>();
            fakeRabbitMqPublisher.When(x => x.Publish(Arg.Any<Publishers.PublishMessageWrapperDto>()))
                .Do(c =>
                {
                    msgGot = c.Arg<PublishMessageWrapperDto>();
                    contentGot = serializer.Deserialize<SampleEvent>(msgGot.Content.ToArray());
                });

            var sut = GetService<IMessagePublisher>();

            // Act
            var id = await sut.Publish(testData.MessageToBePublished);

            // Assert
            msgGot.ShouldSatisfyAllConditions
            (
                () => id.ShouldBe(testData.ExpectedMessageId),
                () => msgGot.ContentType.ShouldBe(testData.ExpectedContentType),
                () => msgGot.CorrelationId.ShouldBe(testData.ExpectedCorrelationId),
                () => msgGot.RoutingKey.ShouldBe(testData.ExpectedRoutingKey),
                () => msgGot.MessageId.ShouldBe(testData.ExpectedMessageId),
                () => msgGot.Headers.ShouldBe(testData.ExpectedHeaders),
                () => msgGot.DeliveryMode.ShouldBe(testData.ExpectedDeliveryMode),
                () => msgGot.Exchange.ShouldBe(testData.ExpectedExchangeName),
                () => msgGot.ExpiryInSeconds.ShouldBe(testData.ExpectedExpriryInSeconds),
                () => contentGot.ShouldBe(testData.ExpectedSampleEvent)
            );

        }

        private IRabbitMqSettings SetupSettings(IRabbitMqSettings settings)
        {
            var fake = GetService<IRabbitMqSettings>();

            fake.AppId.Returns(settings.AppId);
            fake.ConnectionString.Returns(settings.ConnectionString);
            fake.ContentType.Returns(settings.ContentType);
            fake.DefaultTTLInSeconds.Returns(settings.DefaultTTLInSeconds);
            fake.ExchangeName.Returns(settings.ExchangeName);
            fake.ExchangeType.Returns(settings.ExchangeType);
            fake.ImplicitHeaderPrefix.Returns(settings.ImplicitHeaderPrefix);
            fake.MessageTypePrefix.Returns(settings.MessageTypePrefix);
            fake.SkipCreateExchange.Returns(settings.SkipCreateExchange);

            return fake;
        }

        public static IEnumerable<object[]> TestData = new[]
        {
            new object[]
            {
                // minimalistic settings and message
                new PublishTestData
                {
                    GivenUniqueId = Guid.Parse("654bbccd-f30e-4502-9c61-2b11578988db"),
                    MessageToBePublished = new Message<SampleEvent>
                    {
                        Content = new SampleEvent
                        {
                            Name = "test-name"
                        }
                    },
                    ExpectedMessageId = Guid.Parse("654bbccd-f30e-4502-9c61-2b11578988db"),
                    ExpectedSampleEvent = new SampleEvent
                    {
                        Name = "test-name"
                    },
                    ExpectedContentType = "application/json",
                    ExpectedCorrelationId = null,
                    ExpectedRoutingKey = "SampleEvent",
                    ExpectedDeliveryMode = 2,
                    ExpectedHeaders = new Dictionary<string, string>
                    {
                        ["blt-app-id"] = "none",
                        ["blt-msg-type"] = "SampleEvent",
                        ["blt-msg-version"] = "1",
                        ["SampleEvent"] = "none"
                    }
                }
            },
            new object[]
            {
                // set header prefix
                new PublishTestData
                {
                    GivenSettings = new RabbitMqSettings
                    {
                        ImplicitHeaderPrefix = "b-",
                        ExchangeName = "api-order-x",
                        AppId = "api-order",
                        DefaultTTLInSeconds = 60,
                        MessageTypePrefix = "Events."
                    },
                    GivenUniqueId = Guid.Parse("654bbccd-f30e-4502-9c61-2b11578988db"),
                    MessageToBePublished = new Message<SampleEvent>
                    {
                        Content = new SampleEvent
                        {
                            Name = "test-name"
                        }
                    },
                    ExpectedMessageId = Guid.Parse("654bbccd-f30e-4502-9c61-2b11578988db"),
                    ExpectedSampleEvent = new SampleEvent
                    {
                        Name = "test-name"
                    },
                    ExpectedContentType = "application/json",
                    ExpectedCorrelationId = null,
                    ExpectedRoutingKey = "Events.SampleEvent",
                    ExpectedDeliveryMode = 2,
                    ExpectedExchangeName = "api-order-x",
                    ExpectedExpriryInSeconds = 60,
                    ExpectedHeaders = new Dictionary<string, string>
                    {
                        ["b-app-id"] = "api-order",
                        ["b-msg-type"] = "Events.SampleEvent",
                        ["b-msg-version"] = "1",
                        ["Events.SampleEvent"] = "api-order"
                    }
                }
            }
        };


    }

    public record PublishTestData
    {
        internal IRabbitMqSettings GivenSettings { get; init; } = new RabbitMqSettings();
        public Guid GivenUniqueId { get; init; }
        public Message<SampleEvent> MessageToBePublished { get; init; }
        public string ExpectedContentType { get; init; }
        public string ExpectedCorrelationId { get; init; }
        public byte ExpectedDeliveryMode { get; init; }
        public string ExpectedExchangeName { get; init; }
        public Dictionary<string, string> ExpectedHeaders { get; init; }
        public Guid ExpectedMessageId { get; init; }
        public string ExpectedRoutingKey { get; init; }
        public int? ExpectedExpriryInSeconds { get; init; }
        public SampleEvent ExpectedSampleEvent { get; init; }
    }

    public record SampleEvent
    {
        public string Name { get; init; }
    }
}
