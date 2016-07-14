using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Dapper.Hat.SqlServer
{
    sealed class DatabaseConnection : IDatabaseConnection
    {
        readonly SqlConnection _connection;
        readonly int? _defaultCommandTimeout;

        public DatabaseConnection(SqlConnection connection, int? defaultCommandTimeout)
        {
            _connection = connection;
            _defaultCommandTimeout = defaultCommandTimeout;
        }

        public void Dispose()
        {
            _connection.Dispose();
        }

        async public Task<IEnumerable<T>> Query<T>(string sql, IDatabaseCommandParameters param = null, CommandType commandType = CommandType.StoredProcedure,
            int? commandTimeout = null)
        {
            return await _connection.QueryAsync<T>(sql: sql, param: param, commandType: commandType, commandTimeout: commandTimeout ?? _defaultCommandTimeout)
                .ConfigureAwait(false);
        }

        async public Task<IMultiQueryResult> MultiQuery(string sql, IDatabaseCommandParameters param = null, CommandType commandType = CommandType.StoredProcedure,
            int? commandTimeout = null)
        {
            var results = await _connection.QueryMultipleAsync(sql: sql, param: param, commandType: commandType, commandTimeout: commandTimeout ?? _defaultCommandTimeout)
                .ConfigureAwait(false);
            return new MultiQueryResult(results);
        }

        async public Task<int> Execute(string sql, IDatabaseCommandParameters param = null, CommandType commandType = CommandType.StoredProcedure,
            int? commandTimeout = null)
        {
            return await _connection.ExecuteAsync(sql: sql, param: param, commandType: commandType, commandTimeout: commandTimeout ?? _defaultCommandTimeout)
                .ConfigureAwait(false);
        }
    }
}
