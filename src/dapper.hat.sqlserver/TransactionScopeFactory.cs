using System;
using System.Transactions;

namespace Dapper.Hat.SqlServer
{
    /// <summary>
    /// Transaction scope factory implementation
    /// </summary>
    public class TransactionScopeFactory : ITransactionScopeFactory
    {
        /// <summary>
        /// Create transaction scope with specified isolation, async flow setting, scope and timeout.
        /// </summary>
        public ITransactionScope Create(
            IsolationLevel isolationLevel,
            TransactionScopeOption scopeOption,
            bool asyncFlowSupport,
            TimeSpan? timeout
            )
        {
            var transactionOptions = new TransactionOptions
            {
                Timeout = timeout ?? TransactionManager.DefaultTimeout,
                IsolationLevel = isolationLevel
            };

            return new InternalTransactionScope(
                new TransactionScope(
                    scopeOption,
                    transactionOptions,
                    asyncFlowSupport ? TransactionScopeAsyncFlowOption.Enabled : TransactionScopeAsyncFlowOption.Suppress
                    )
                );
        }
    }
}