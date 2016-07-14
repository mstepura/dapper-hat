using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Dapper.Hat.SqlServer.Testing
{
    sealed class ValidatingDatabaseConnection : IDatabaseConnection
    {
        readonly SqlConnection _sqlConnection;

        public ValidatingDatabaseConnection(SqlConnection sqlConnection)
        {
            _sqlConnection = sqlConnection;
        }

        public void Dispose()
        {
            _sqlConnection.Dispose();
            GC.SuppressFinalize(this);
        }

#if DEBUG
        ~ValidatingDatabaseConnection()
        {
            Debug.Fail("ValidatingDatabaseConnection has not been properly disposed");
        }
#endif

        async public Task<IEnumerable<T>> Query<T>(string sql, IDatabaseCommandParameters param = null, CommandType commandType = CommandType.StoredProcedure, int? commandTimeout = null)
        {
            return await Task.Run(() =>
                _sqlConnection.QueryValidate<T>(
                    queryText: sql,
                    parameters: param,
                    commandType: commandType
                    )
                ).ConfigureAwait(false);
        }

        async public Task<int> Execute(string sql, IDatabaseCommandParameters param = null, CommandType commandType = CommandType.StoredProcedure, int? commandTimeout = null)
        {
            return await Task.Run(() =>
                _sqlConnection.ExecuteValidate(
                    commandText: sql,
                    parameters: param,
                    commandType: commandType
                    )
                ).ConfigureAwait(false);
        }

        async public Task<IMultiQueryResult> MultiQuery(string sql, IDatabaseCommandParameters param = null, CommandType commandType = CommandType.StoredProcedure, int? commandTimeout = null)
        {
            return await Task.Run(() =>
                _sqlConnection.QueryMultipleValidate(
                    queryText: sql,
                    parameters: param,
                    commandType: commandType
                    )
                ).ConfigureAwait(false);
        }
    }
}
