using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Dapper.Hat.SqlServer
{
    /// <summary>
    /// Database connection interface.
    /// </summary>
    public interface IDatabaseConnection : IDisposable
    {
        /// <summary>
        /// Executes the command which returns recordset.
        /// </summary>
        Task<IEnumerable<T>> Query<T>(string sql, IDatabaseCommandParameters param = null,
            CommandType commandType = CommandType.StoredProcedure, int? commandTimeout = null);

        /// <summary>
        /// Executes the command which returns multiple recordsets.
        /// </summary>
        Task<IMultiQueryResult> MultiQuery(string sql, IDatabaseCommandParameters param = null,
            CommandType commandType = CommandType.StoredProcedure, int? commandTimeout = null);

        /// <summary>
        /// Executes the command which does not return recordsets.
        /// </summary>
        /// <returns>Number of rows affected</returns>
        Task<int> Execute(string sql, IDatabaseCommandParameters param = null,
            CommandType commandType = CommandType.StoredProcedure, int? commandTimeout = null);
    }
}