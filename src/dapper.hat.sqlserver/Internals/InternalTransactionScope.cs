using System.Transactions;

namespace Dapper.Hat.SqlServer
{
    sealed class InternalTransactionScope : ITransactionScope
    {
        readonly TransactionScope _scope;

        public InternalTransactionScope(TransactionScope scope)
        {
            _scope = scope;
        }

        public void Dispose()
        {
            _scope.Dispose();
        }

        public void Complete()
        {
            _scope.Complete();
        }
    }
}
