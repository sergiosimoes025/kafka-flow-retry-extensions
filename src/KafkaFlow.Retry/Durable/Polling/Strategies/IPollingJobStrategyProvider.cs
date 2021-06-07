﻿namespace KafkaFlow.Retry.Durable.Polling.Strategies
{
    internal interface IPollingJobStrategyProvider
    {
        IPollingJobStrategy GetPollingJobStrategy(PollingStrategy strategy);
    }
}