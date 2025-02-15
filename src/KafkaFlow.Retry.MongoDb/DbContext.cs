﻿namespace KafkaFlow.Retry.MongoDb
{
    using KafkaFlow.Retry.MongoDb.Model;
    using MongoDB.Driver;

    internal sealed class DbContext
    {
        private readonly IMongoDatabase database;
        private readonly MongoDbSettings mongoDbSettings;

        public DbContext(MongoDbSettings mongoDbSettings, IMongoClient mongoClient)
        {
            this.mongoDbSettings = mongoDbSettings;
            this.MongoClient = mongoClient;

            this.database = mongoClient.GetDatabase(this.mongoDbSettings.DatabaseName);
        }

        public IMongoClient MongoClient { get; }
        public IMongoCollection<RetryQueueItemDbo> RetryQueueItems => database.GetCollection<RetryQueueItemDbo>(this.mongoDbSettings.RetryQueueItemCollectionName);

        public IMongoCollection<RetryQueueDbo> RetryQueues => database.GetCollection<RetryQueueDbo>(this.mongoDbSettings.RetryQueueCollectionName);
    }
}
