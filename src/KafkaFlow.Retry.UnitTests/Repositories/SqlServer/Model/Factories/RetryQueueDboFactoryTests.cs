﻿namespace KafkaFlow.Retry.UnitTests.Repositories.SqlServer.Model.Factories
{
    using System;
    using FluentAssertions;
    using global::KafkaFlow.Retry.Durable.Common;
    using global::KafkaFlow.Retry.Durable.Repository.Actions.Create;
    using global::KafkaFlow.Retry.Durable.Repository.Model;
    using global::KafkaFlow.Retry.SqlServer.Model;
    using global::KafkaFlow.Retry.SqlServer.Model.Factories;
    using Xunit;

    public class RetryQueueDboFactoryTests
    {
        private readonly RetryQueueDboFactory factory = new RetryQueueDboFactory();

        [Fact]
        public void RetryQueueDboFactory_Create_Success()
        {
            // Arrange
            var saveToQueueInput = new SaveToQueueInput(
                new RetryQueueItemMessage("topicName", new byte[] { 1, 3 }, new byte[] { 2, 4, 6 }, 3, 21, DateTime.UtcNow),
                "searchGroupKey",
                "queueGroupKey",
                RetryQueueStatus.Active,
                RetryQueueItemStatus.Done,
                SeverityLevel.High,
                DateTime.UtcNow,
                DateTime.UtcNow,
                DateTime.UtcNow,
                3,
                "description");

            // Act
            var result = factory.Create(saveToQueueInput);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(RetryQueueDbo));
        }

        [Fact]
        public void RetryQueueDboFactory_Create_WithoutSaveToQueueInput_ThrowsException()
        {
            // Arrange
            SaveToQueueInput saveToQueueInput = null;

            // Act
            Action act = () => factory.Create(saveToQueueInput);

            // Assert
            act.Should().Throw<ArgumentNullException>();
        }
    }
}