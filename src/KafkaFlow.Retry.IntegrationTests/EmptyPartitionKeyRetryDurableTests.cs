namespace KafkaFlow.Retry.IntegrationTests
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using KafkaFlow.Retry.IntegrationTests.Core.Bootstrappers.Fixtures;
    using KafkaFlow.Retry.IntegrationTests.Core.Messages;
    using KafkaFlow.Retry.IntegrationTests.Core.Producers;
    using KafkaFlow.Retry.IntegrationTests.Core.Storages;
    using KafkaFlow.Retry.IntegrationTests.Core.Storages.Assertion;
    using KafkaFlow.Retry.IntegrationTests.Core.Storages.Repositories;
    using Microsoft.Extensions.DependencyInjection;
    using Xunit;

    [Collection("BootstrapperHostCollection")]
    public class EmptyPartitionKeyRetryDurableTests
    {
        private readonly IRepositoryProvider repositoryProvider;
        private readonly IServiceProvider serviceProvider;

        public EmptyPartitionKeyRetryDurableTests(BootstrapperHostFixture bootstrapperHostFixture)
        {
            this.serviceProvider = bootstrapperHostFixture.ServiceProvider;
            this.repositoryProvider = bootstrapperHostFixture.ServiceProvider.GetRequiredService<IRepositoryProvider>();
            InMemoryAuxiliarStorage<RetryDurableTestMessage>.Clear();
            InMemoryAuxiliarStorage<RetryDurableTestMessage>.ThrowException = true;
        }

        public static IEnumerable<object[]> EmptyKeyScenarios()
        {
            yield return new object[]
            {
                RepositoryType.MongoDb,
                typeof(IMessageProducer<RetryDurableGuaranteeOrderedConsumptionMongoDbProducer>),
                typeof(RetryDurableGuaranteeOrderedConsumptionPhysicalStorageAssert)
            };
            yield return new object[]
            {
                RepositoryType.SqlServer,
                typeof(IMessageProducer<RetryDurableGuaranteeOrderedConsumptionSqlServerProducer>),
                typeof(RetryDurableGuaranteeOrderedConsumptionPhysicalStorageAssert)
            };
        }

        [Theory]
        [MemberData(nameof(EmptyKeyScenarios))]
        internal async Task EmptyKeyRetryDurableTest(
            RepositoryType repositoryType,
            Type producerType,
            Type physicalStorageType)
        {
            // Arrange
            var numberOfMessagesToBeProduced = 2;
            var numberOfTimesThatEachMessageIsTriedWhenDone = 1;
            var numberOfMessagesByEachSameKey = 1;
            var numberOfTimesThatEachMessageIsTriedBeforeDurable = 4;
            var numberOfTimesThatEachMessageIsTriedDuringDurable = 1;
            var producer = this.serviceProvider.GetRequiredService(producerType) as IMessageProducer;
            var physicalStorageAssert = this.serviceProvider.GetRequiredService(physicalStorageType) as IPhysicalStorageAssert;
            var messages = new List<RetryDurableTestMessage>();
            for (int i = 0; i < numberOfMessagesToBeProduced; i++)
            {
                messages.Add(new RetryDurableTestMessage { Key = string.Empty, Value = $"Message_{i + 1}" });
            }

            await this.repositoryProvider.GetRepositoryOfType(repositoryType).CleanDatabaseAsync().ConfigureAwait(false);

            // Act
            foreach (var message in messages)
            {
                await producer.ProduceAsync(message.Key, message).ConfigureAwait(false);
            }

            // Assert - Creation
            var messageToValidate = messages[0];

            await InMemoryAuxiliarStorage<RetryDurableTestMessage>
                .AssertCountMessageAsync(messageToValidate, numberOfTimesThatEachMessageIsTriedBeforeDurable)
                .ConfigureAwait(false);

            //foreach (var message in messages)
            //{
            //    await physicalStorageAssert
            //        .AssertRetryDurableMessageCreationAsync(repositoryType, message, numberOfMessagesByEachSameKey)
            //        .ConfigureAwait(false);
            //}

            // Assert - Retrying
            InMemoryAuxiliarStorage<RetryDurableTestMessage>.Clear();

            await InMemoryAuxiliarStorage<RetryDurableTestMessage>
                .AssertCountMessageAsync(messageToValidate, numberOfTimesThatEachMessageIsTriedDuringDurable)
                .ConfigureAwait(false);

            //foreach (var message in messages)
            //{
            //    await physicalStorageAssert
            //        .AssertRetryDurableMessageRetryingAsync(repositoryType, message, numberOfTimesThatEachMessageIsTriedDuringDurable)
            //        .ConfigureAwait(false);
            //}

            // Assert - Done
            InMemoryAuxiliarStorage<RetryDurableTestMessage>.ThrowException = false;
            InMemoryAuxiliarStorage<RetryDurableTestMessage>.Clear();

            foreach (var message in messages)
            {
                await InMemoryAuxiliarStorage<RetryDurableTestMessage>
                    .AssertCountMessageAsync(message, numberOfTimesThatEachMessageIsTriedWhenDone)
                    .ConfigureAwait(false);
            }

            //foreach (var message in messages)
            //{
            //    await physicalStorageAssert
            //        .AssertRetryDurableMessageDoneAsync(repositoryType, message)
            //        .ConfigureAwait(false);
            //}
        }
    }
}