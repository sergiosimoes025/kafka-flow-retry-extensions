﻿namespace KafkaFlow.Retry.Durable
{
    using Dawn;
    using KafkaFlow.Retry.Durable.Polling.Strategies;

    internal class KafkaRetryDurablePollingDefinition
    {
        public KafkaRetryDurablePollingDefinition(
            bool enabled,
            string cronExpression,
            int fetchSize,
            int expirationIntervalFactor,
            PollingJobStrategyType pollingJobStrategy)
        {
            if (enabled)
            {
                Guard
                    .Argument(Quartz.CronExpression.IsValidExpression(cronExpression), nameof(cronExpression))
                    .True("A valid cron expression is required when the polling is enabled.");
            }

            Guard.Argument(fetchSize, nameof(fetchSize)).Positive();
            Guard.Argument(expirationIntervalFactor, nameof(expirationIntervalFactor)).Positive();

            this.CronExpression = cronExpression;
            this.Enabled = enabled;
            this.FetchSize = fetchSize;
            this.ExpirationIntervalFactor = expirationIntervalFactor;
            this.PollingJobStrategy = pollingJobStrategy;
        }

        public string CronExpression { get; }

        public bool Enabled { get; }

        public int ExpirationIntervalFactor { get; }

        public int FetchSize { get; }

        public PollingJobStrategyType PollingJobStrategy { get; }
    }
}