﻿namespace KafkaFlow.Retry.UnitTests.KafkaFlow.Retry.Durable.Polling
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using FluentAssertions;
    using global::KafkaFlow.Retry.Durable.Definitions;
    using global::KafkaFlow.Retry.Durable.Encoders;
    using global::KafkaFlow.Retry.Durable.Polling;
    using global::KafkaFlow.Retry.Durable.Repository;
    using global::KafkaFlow.Retry.Durable.Repository.Adapters;
    using Moq;
    using Xunit;

    [ExcludeFromCodeCoverage]
    public class QueueTrackerTests
    {
        public readonly static IEnumerable<object[]> DataTest = new List<object[]>
        {
            new object[]
            {
                null,
                Mock.Of<ILogHandler>(),
                Mock.Of<IMessageHeadersAdapter>(),
                Mock.Of<IMessageAdapter>(),
                Mock.Of<IUtf8Encoder>(),
                Mock.Of<IMessageProducer>(),
                retryDurablePollingDefinition
            },
            new object[]
            {
                Mock.Of<IRetryDurableQueueRepository>(),
                null,
                Mock.Of<IMessageHeadersAdapter>(),
                Mock.Of<IMessageAdapter>(),
                Mock.Of<IUtf8Encoder>(),
                Mock.Of<IMessageProducer>(),
                retryDurablePollingDefinition
            },
            new object[]
            {
                Mock.Of<IRetryDurableQueueRepository>(),
                Mock.Of<ILogHandler>(),
                null,
                Mock.Of<IMessageAdapter>(),
                Mock.Of<IUtf8Encoder>(),
                Mock.Of<IMessageProducer>(),
                retryDurablePollingDefinition
            },
            new object[]
            {
                Mock.Of<IRetryDurableQueueRepository>(),
                Mock.Of<ILogHandler>(),
                Mock.Of<IMessageHeadersAdapter>(),
                null,
                Mock.Of<IUtf8Encoder>(),
                Mock.Of<IMessageProducer>(),
                retryDurablePollingDefinition
            },
            new object[]
            {
                Mock.Of<IRetryDurableQueueRepository>(),
                Mock.Of<ILogHandler>(),
                Mock.Of<IMessageHeadersAdapter>(),
                Mock.Of<IMessageAdapter>(),
                null,
                Mock.Of<IMessageProducer>(),
                retryDurablePollingDefinition
            },
            new object[]
            {
                Mock.Of<IRetryDurableQueueRepository>(),
                Mock.Of<ILogHandler>(),
                Mock.Of<IMessageHeadersAdapter>(),
                Mock.Of<IMessageAdapter>(),
                Mock.Of<IUtf8Encoder>(),
                null,
                retryDurablePollingDefinition
            },
            new object[]
            {
                Mock.Of<IRetryDurableQueueRepository>(),
                Mock.Of<ILogHandler>(),
                Mock.Of<IMessageHeadersAdapter>(),
                Mock.Of<IMessageAdapter>(),
                Mock.Of<IUtf8Encoder>(),
                Mock.Of<IMessageProducer>(),
                null
            }
        };

        private static readonly RetryDurablePollingDefinition retryDurablePollingDefinition = new RetryDurablePollingDefinition(true, "*/30 * * ? * *", 10, 100, "id");

        [Theory]
        [MemberData(nameof(DataTest))]
        public void QueueTracker_Ctor_WithArgumentNull_ThrowsException(
            object retryDurableQueueRepository,
            object logHandler,
            object messageHeadersAdapter,
            object messageAdapter,
            object utf8Encoder,
            object retryDurableMessageProducer,
            object retryDurablePollingDefinition)
        {
            Action act = () => new QueueTracker((IRetryDurableQueueRepository)retryDurableQueueRepository,
                (ILogHandler)logHandler, (IMessageHeadersAdapter)messageHeadersAdapter,
                (IMessageAdapter)messageAdapter, (IUtf8Encoder)utf8Encoder,
                (IMessageProducer)retryDurableMessageProducer, (RetryDurablePollingDefinition)retryDurablePollingDefinition);

            act.Should().Throw<ArgumentNullException>();
        }
    }
}