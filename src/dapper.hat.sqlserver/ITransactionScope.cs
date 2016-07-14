using System;

namespace Dapper.Hat.SqlServer
{
    /// <summary>
    /// Transaction scope interface
    /// </summary>
    public interface ITransactionScope : IDisposable
    {
        /// <summary>
        /// Commits the transaction.
        /// If this method is not called before the scope is disposed, transaction is rolled back.
        /// </summary>
        void Complete();
    }
}