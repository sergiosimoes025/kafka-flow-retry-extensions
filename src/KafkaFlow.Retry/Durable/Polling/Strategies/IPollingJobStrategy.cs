﻿
namespace KafkaFlow.Retry.Durable.Polling.Strategies
{
    using System.Threading.Tasks;
    using KafkaFlow.Producers;
    using KafkaFlow.Retry.Durable.Repository;

    internal interface IPollingJobStrategy
    {
        PollingJobStrategyType PollingJobStrategyType { get; }

        Task ExecuteAsync(
            IKafkaRetryDurableQueueRepository queueStorage,
            IMessageProducer messageProducer,
            KafkaRetryDurablePollingDefinition kafkaRetryDurablePollingDefinition);
    }
}
