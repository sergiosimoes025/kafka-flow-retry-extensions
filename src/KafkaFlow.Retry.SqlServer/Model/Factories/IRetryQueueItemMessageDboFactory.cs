﻿namespace KafkaFlow.Retry.SqlServer.Model.Factories
{
    using KafkaFlow.Retry.Durable.Repository.Model;

    internal interface IRetryQueueItemMessageDboFactory
    {
        RetryQueueItemMessageDbo Create(RetryQueueItemMessage retryQueueItemMessage, long retryQueueItemId);
    }
}
