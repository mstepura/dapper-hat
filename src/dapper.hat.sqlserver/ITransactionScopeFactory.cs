using System;
using System.Transactions;

namespace Dapper.Hat.SqlServer
{
    /// <summary>
    /// Transaction scope factory interface
    /// </summary>
    public interface ITransactionScopeFactory
    {
        /// <summary>
        /// Creates transaction scope.
        /// </summary>
        /// <param name="isolationLevel">Isolation level</param>
        /// <param name="scopeOption">Transaction scope options.</param>
        /// <param name="asyncFlowSupport">
        /// Specifies that transaction flow across thread continuations is enabled, <seealso cref="TransactionScopeAsyncFlowOption"/>.
        /// </param>
        /// <param name="timeout">Transaction timeout, if null - default timeout specified in application or machine configuration is used.</param>
        ITransactionScope Create(
            IsolationLevel isolationLevel = IsolationLevel.ReadCommitted,
            TransactionScopeOption scopeOption = TransactionScopeOption.Required,
            bool asyncFlowSupport = true,
            TimeSpan? timeout = null
            );
    }
}
