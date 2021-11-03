﻿namespace KafkaFlow.Retry.Durable
{
    using System;
    using System.Threading.Tasks;
    using KafkaFlow.Retry.Durable.Serializers;

    internal class RetryDurableConsumerNewtonsoftJsonSerializerMiddleware : IMessageMiddleware
    {
        private readonly INewtonsoftJsonSerializer newtonsoftJsonSerializer;
        private readonly Type type;

        public RetryDurableConsumerNewtonsoftJsonSerializerMiddleware(INewtonsoftJsonSerializer newtonsoftJsonSerializer, Type type)
        {
            this.newtonsoftJsonSerializer = newtonsoftJsonSerializer; this.type = type;
        }

        public async Task Invoke(IMessageContext context, MiddlewareDelegate next)
        {
            await next(context.SetMessage(context.Message.Key, this.newtonsoftJsonSerializer.DeserializeObject((string)context.Message.Value, type))).ConfigureAwait(false);
        }
    }
}