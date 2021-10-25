﻿namespace KafkaFlow.Retry.IntegrationTests.Core.Storages.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal class RepositoryProvider : IRepositoryProvider
    {
        private readonly IEnumerable<IRepository> repositories;

        public RepositoryProvider(IEnumerable<IRepository> repositories)
        {
            this.repositories = repositories;
        }

        public IRepository GetRepositoryOfType(Type repositoryType)
        {
            return this.repositories.Single(r => r.GetType().Equals(repositoryType));
        }
    }
}