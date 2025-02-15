﻿namespace KafkaFlow.Retry.Durable.Repository.Actions.Read
{
    using System.Diagnostics.CodeAnalysis;
    using Dawn;

    [ExcludeFromCodeCoverage]
    public class QueueNewestItemsResult
    {
        public QueueNewestItemsResult(QueueNewestItemsResultStatus status)
        {
            Guard.Argument(status, nameof(status)).NotDefault();

            this.Status = status;
        }

        public QueueNewestItemsResultStatus Status { get; }
    }
}