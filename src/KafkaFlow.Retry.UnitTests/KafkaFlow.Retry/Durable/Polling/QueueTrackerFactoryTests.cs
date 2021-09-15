﻿using System;
using System.Collections.Generic;
using FluentAssertions;
using KafkaFlow.Retry.Durable.Definitions;
using KafkaFlow.Retry.Durable.Encoders;
using KafkaFlow.Retry.Durable.Polling;
using KafkaFlow.Retry.Durable.Repository;
using KafkaFlow.Retry.Durable.Repository.Adapters;
using Moq;
using Xunit;

namespace KafkaFlow.Retry.UnitTests.KafkaFlow.Retry.Durable.Polling
{
    public class QueueTrackerFactoryTests
    {
        public static IEnumerable<object[]> DataTest = new List<object[]>
        {
            new object[]
            {
                null,
                Mock.Of<ILogHandler>() ,
                Mock.Of<IMessageHeadersAdapter>() ,
                Mock.Of<IMessageAdapter>() ,
                Mock.Of<IUtf8Encoder>() ,
                Mock.Of<IMessageProducer>() ,
                Mock.Of<IRetryDurablePollingDefinition>()
            },
             new object[]
            {
                Mock.Of<IRetryDurableQueueRepository>(),
                null ,
                Mock.Of<IMessageHeadersAdapter>() ,
                Mock.Of<IMessageAdapter>() ,
                Mock.Of<IUtf8Encoder>() ,
                Mock.Of<IMessageProducer>() ,
                Mock.Of<IRetryDurablePollingDefinition>()
            },
              new object[]
            {
                Mock.Of<IRetryDurableQueueRepository>(),
                Mock.Of<ILogHandler>() ,
                null ,
                Mock.Of<IMessageAdapter>() ,
                Mock.Of<IUtf8Encoder>() ,
                Mock.Of<IMessageProducer>() ,
                Mock.Of<IRetryDurablePollingDefinition>()
            },
               new object[]
            {
                Mock.Of<IRetryDurableQueueRepository>(),
                Mock.Of<ILogHandler>() ,
                Mock.Of<IMessageHeadersAdapter>() ,
                null ,
                Mock.Of<IUtf8Encoder>() ,
                Mock.Of<IMessageProducer>() ,
                Mock.Of<IRetryDurablePollingDefinition>()
            },
                new object[]
            {
                Mock.Of<IRetryDurableQueueRepository>(),
                Mock.Of<ILogHandler>() ,
                Mock.Of<IMessageHeadersAdapter>() ,
                Mock.Of<IMessageAdapter>() ,
                null ,
                Mock.Of<IMessageProducer>() ,
                Mock.Of<IRetryDurablePollingDefinition>()
            },
                 new object[]
            {
                Mock.Of<IRetryDurableQueueRepository>(),
                Mock.Of<ILogHandler>() ,
                Mock.Of<IMessageHeadersAdapter>() ,
                Mock.Of<IMessageAdapter>() ,
                Mock.Of<IUtf8Encoder>() ,
                null,
                Mock.Of<IRetryDurablePollingDefinition>()
            },
                  new object[]
            {
                Mock.Of<IRetryDurableQueueRepository>(),
                Mock.Of<ILogHandler>() ,
                Mock.Of<IMessageHeadersAdapter>() ,
                Mock.Of<IMessageAdapter>() ,
                Mock.Of<IUtf8Encoder>() ,
                Mock.Of<IMessageProducer>() ,
                null
            }
        };

        [Fact]
        public void QueueTrackerFactory_Create_Success()
        {
            // Arrange
            var factory = new QueueTrackerFactory(
                Mock.Of<IRetryDurableQueueRepository>(),
                Mock.Of<ILogHandler>(),
                Mock.Of<IMessageHeadersAdapter>(),
                Mock.Of<IMessageAdapter>(),
                Mock.Of<IUtf8Encoder>(),
                Mock.Of<IMessageProducer>(),
                Mock.Of<IRetryDurablePollingDefinition>());

            // Act
            var queueTracker = factory.Create();

            // Arrange
            queueTracker.Should().NotBeNull();
        }

        [Theory]
        [MemberData(nameof(DataTest))]
        public void QueueTrackerFactory_Ctor_WithArgumentNull_ThrowsException(
            object retryDurableQueueRepository,
            object logHandler,
            object messageHeadersAdapter,
            object messageAdapter,
            object utf8Encoder,
            object retryDurableMessageProducer,
            object retryDurablePollingDefinition)
        {
            // Arrange & Act
            Action act = () => new QueueTrackerFactory(
            (IRetryDurableQueueRepository)retryDurableQueueRepository,
            (ILogHandler)logHandler,
            (IMessageHeadersAdapter)messageHeadersAdapter,
            (IMessageAdapter)messageAdapter,
            (IUtf8Encoder)utf8Encoder,
            (IMessageProducer)retryDurableMessageProducer,
            (IRetryDurablePollingDefinition)retryDurablePollingDefinition);

            // Assert
            act.Should().Throw<ArgumentNullException>();
        }
    }
}