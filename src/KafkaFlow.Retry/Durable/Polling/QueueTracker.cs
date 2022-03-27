﻿namespace KafkaFlow.Retry.Durable.Polling
{
    using System.Threading;
    using Dawn;
    using KafkaFlow.Retry.Durable.Definitions;
    using KafkaFlow.Retry.Durable.Encoders;
    using KafkaFlow.Retry.Durable.Repository;
    using KafkaFlow.Retry.Durable.Repository.Adapters;
    using Quartz;
    using Quartz.Impl;

    internal class QueueTracker
    {
        private static readonly object internalLock = new object();
        private readonly ILogHandler logHandler;
        private readonly IMessageAdapter messageAdapter;
        private readonly IMessageHeadersAdapter messageHeadersAdapter;
        private readonly IMessageProducer retryDurableMessageProducer;
        private readonly RetryDurablePollingDefinition retryDurablePollingDefinition;
        private readonly IRetryDurableQueueRepository retryDurableQueueRepository;
        private readonly IUtf8Encoder utf8Encoder;
        private readonly bool waitForJobsToComplete = true;
        private IScheduler scheduler;

        public QueueTracker(
            IRetryDurableQueueRepository retryDurableQueueRepository,
            ILogHandler logHandler,
            IMessageHeadersAdapter messageHeadersAdapter,
            IMessageAdapter messageAdapter,
            IUtf8Encoder utf8Encoder,
            IMessageProducer retryDurableMessageProducer,
            RetryDurablePollingDefinition retryDurablePollingDefinition
        )
        {
            Guard.Argument(retryDurableQueueRepository).NotNull();
            Guard.Argument(logHandler).NotNull();
            Guard.Argument(messageHeadersAdapter).NotNull();
            Guard.Argument(messageAdapter).NotNull();
            Guard.Argument(utf8Encoder).NotNull();
            Guard.Argument(retryDurableMessageProducer).NotNull();
            Guard.Argument(retryDurablePollingDefinition).NotNull();

            this.retryDurableQueueRepository = retryDurableQueueRepository;
            this.logHandler = logHandler;
            this.messageHeadersAdapter = messageHeadersAdapter;
            this.messageAdapter = messageAdapter;
            this.utf8Encoder = utf8Encoder;
            this.retryDurableMessageProducer = retryDurableMessageProducer;
            this.retryDurablePollingDefinition = retryDurablePollingDefinition;
        }

        private bool IsSchedulerActive
            => this.scheduler is object && this.scheduler.IsStarted && !this.scheduler.IsShutdown;

        internal void ScheduleJob(CancellationToken cancellationToken = default)
        {
            Guard.Argument(this.scheduler).Null(s => "Scheduler was already started. Please call this method just once.");

            lock (internalLock)
            {
                this.scheduler = StdSchedulerFactory.GetDefaultScheduler(cancellationToken).GetAwaiter().GetResult();
            }

            this.scheduler
                .Start(cancellationToken)
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();

            JobDataMap dataMap = new JobDataMap
            {
                { QueuePollingJobConstants.RetryDurableQueueRepository, this.retryDurableQueueRepository },
                { QueuePollingJobConstants.RetryDurableMessageProducer, this.retryDurableMessageProducer },
                { QueuePollingJobConstants.RetryDurablePollingDefinition, this.retryDurablePollingDefinition },
                { QueuePollingJobConstants.LogHandler, this.logHandler },
                { QueuePollingJobConstants.MessageHeadersAdapter, this.messageHeadersAdapter },
                { QueuePollingJobConstants.MessageAdapter, this.messageAdapter },
                { QueuePollingJobConstants.Utf8Encoder, this.utf8Encoder }
            };

            IJobDetail job = JobBuilder
                .Create<QueuePollingJob>()
                .SetJobData(dataMap)
                .Build();

            ITrigger trigger = TriggerBuilder
                .Create()
                .WithIdentity($"pollingJob_{this.retryDurablePollingDefinition.Id}", "queueTrackerGroup")
                .WithCronSchedule(this.retryDurablePollingDefinition.CronExpression)
                .StartNow()
                .WithPriority(1)
                .Build();

            this.scheduler
                .ScheduleJob(job, trigger, cancellationToken)
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();

            this.logHandler.Info(
                "PollingJob Schedule",
                new
                {
                    PollingId = this.retryDurablePollingDefinition.Id,
                    CronExpression = this.retryDurablePollingDefinition.CronExpression
                });
        }

        internal void Shutdown(CancellationToken cancellationToken = default)
        {
            lock (internalLock)
            {
                if (this.IsSchedulerActive)
                {
                    this.scheduler
                        .Shutdown(waitForJobsToComplete, cancellationToken)
                        .ConfigureAwait(false)
                        .GetAwaiter()
                        .GetResult();
                }
            }

            this.logHandler.Info("PollingJob Shutdown", new { });
        }
    }
}