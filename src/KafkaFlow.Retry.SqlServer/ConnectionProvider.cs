﻿namespace KafkaFlow.Retry.SqlServer
{
    using Dawn;

    internal sealed class ConnectionProvider : IConnectionProvider
    {
        public IDbConnection Create(SqlServerDbSettings sqlServerDbSettings)
        {
            Guard.Argument(sqlServerDbSettings).NotNull();

            return new DbConnectionContext(sqlServerDbSettings, false);
        }

        public IDbConnectionWithinTransaction CreateWithinTransaction(SqlServerDbSettings sqlServerDbSettings)
        {
            Guard.Argument(sqlServerDbSettings).NotNull();

            return new DbConnectionContext(sqlServerDbSettings, true);
        }
    }
}