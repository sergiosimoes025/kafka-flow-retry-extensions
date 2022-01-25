namespace KafkaFlow.Retry.UnitTests.Durable.Repository
{
    using global::KafkaFlow.Retry.Durable.Definitions;
    using global::KafkaFlow.Retry.Durable.Encoders;
    using global::KafkaFlow.Retry.Durable.Repository;
    using global::KafkaFlow.Retry.Durable.Repository.Actions.Create;
    using global::KafkaFlow.Retry.Durable.Repository.Actions.Read;
    using global::KafkaFlow.Retry.Durable.Repository.Adapters;
    using global::KafkaFlow.Retry.Durable.Repository.Model;
    using Moq;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Xunit;

    public class RetryDurableQueueRepositoryTests
    {
        private readonly Mock<IUpdateRetryQueueItemHandler> mockIUpdateRetryQueueItemHandler;
        private readonly Mock<IMessageAdapter> mockMessageAdapter;
        private readonly Mock<IMessageHeadersAdapter> mockMessageHeadersAdapter;
        private readonly MockRepository mockRepository;
        private readonly Mock<IRetryDurableQueueRepositoryProvider> mockRetryDurableQueueRepositoryProvider;
        private readonly Mock<IUtf8Encoder> mockUtf8Encoder;
        private readonly RetryDurablePollingDefinition retryDurablePollingDefinition;
        private readonly RetryDurableQueueRepository retryDurableQueueRepository;

        public RetryDurableQueueRepositoryTests()
        {
            this.mockRepository = new MockRepository(MockBehavior.Strict);

            this.mockRetryDurableQueueRepositoryProvider = this.mockRepository.Create<IRetryDurableQueueRepositoryProvider>();
            this.mockIUpdateRetryQueueItemHandler = this.mockRepository.Create<IUpdateRetryQueueItemHandler>();
            this.mockMessageHeadersAdapter = this.mockRepository.Create<IMessageHeadersAdapter>();
            this.mockMessageAdapter = this.mockRepository.Create<IMessageAdapter>();
            this.mockUtf8Encoder = this.mockRepository.Create<IUtf8Encoder>();
            this.retryDurablePollingDefinition = new RetryDurablePollingDefinition(true, "0 0 0 ? * * *", 1, 1, "some_id");

            retryDurableQueueRepository = new RetryDurableQueueRepository(
                this.mockRetryDurableQueueRepositoryProvider.Object,
                new List<IUpdateRetryQueueItemHandler> { this.mockIUpdateRetryQueueItemHandler.Object },
                this.mockMessageHeadersAdapter.Object,
                this.mockMessageAdapter.Object,
                this.mockUtf8Encoder.Object,
                this.retryDurablePollingDefinition);
        }

        [Theory]
        [InlineData("key", "value")]
        [InlineData(null, "value")]
        [InlineData("", "value")]
        public async Task AddIfQueueExistsAsync_WithValidMessage_ReturnResultStatusAdded(string messageKey, string messageValue)
        {
            // Arrange
            byte[] messageKeyBytes = null;
            if (messageKey is object)
            {
                messageKeyBytes = Encoding.ASCII.GetBytes(messageKey);
            }

            var messageValueBytes = Encoding.ASCII.GetBytes(messageValue);
            AddIfQueueExistsResultStatus addedIfQueueExistsResultStatus = AddIfQueueExistsResultStatus.Added;
            Mock<IConsumerContext> mockIConsumerContext = new Mock<IConsumerContext>();
            mockIConsumerContext
                .SetupGet(c => c.Topic)
                .Returns("topic");
            mockIConsumerContext
                .SetupGet(c => c.Partition)
                .Returns(1);
            mockIConsumerContext
                .SetupGet(c => c.Offset)
                .Returns(2);
            mockIConsumerContext
                .SetupGet(c => c.MessageTimestamp)
                .Returns(new DateTime(2022, 01, 01));

            Mock<IMessageContext> mockIMessageContext = new Mock<IMessageContext>();
            mockIMessageContext
                    .Setup(c => c.ConsumerContext)
                    .Returns(mockIConsumerContext.Object);
            mockIMessageContext
                    .Setup(c => c.Message)
                    .Returns(new Message(messageKeyBytes, messageValueBytes));

            mockMessageAdapter
                .Setup(mes => mes.AdaptMessageToRepository(It.IsAny<object>()))
                .Returns(messageValueBytes);

            mockMessageHeadersAdapter
                .Setup(mes => mes.AdaptMessageHeadersToRepository(It.IsAny<IMessageHeaders>()))
                .Returns(Enumerable.Empty<MessageHeader>());

            if (messageKey is object)
            {
                mockUtf8Encoder
                    .Setup(enc => enc.Decode(It.IsAny<byte[]>()))
                    .Returns(messageKey);
            }

            mockRetryDurableQueueRepositoryProvider
                .Setup(rep => rep.CheckQueueAsync(It.IsAny<CheckQueueInput>()))
                .ReturnsAsync(new CheckQueueResult(CheckQueueResultStatus.Exists));
            mockRetryDurableQueueRepositoryProvider
                .Setup(rep => rep.SaveToQueueAsync(It.IsAny<SaveToQueueInput>()))
                .ReturnsAsync(new SaveToQueueResult(SaveToQueueResultStatus.Added));

            // Act
            var result = await retryDurableQueueRepository.AddIfQueueExistsAsync(
                mockIMessageContext.Object);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(addedIfQueueExistsResultStatus, result.Status);
            this.mockRepository.VerifyAll();
        }
    }
}