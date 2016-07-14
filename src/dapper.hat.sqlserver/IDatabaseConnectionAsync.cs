using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace PartsTrader.Common.Data.Dapper
{
    /// <summary>
    /// Database connection interface, asynchronous version
    /// </summary>
    public interface IDatabaseConnectionAsync : IDisposable
    {
        /// <summary>
        /// Execute the command which returns single recordset.
        /// </summary>
        /// <typeparam name="T">Class to map record type</typeparam>
        /// <returns>Typed recordset</returns>
        Task<IEnumerable<T>> Query<T>(string sql, IDatabaseCommandParameters param = null,
            CommandType commandType = CommandType.StoredProcedure, int? commandTimeout = null);

        /// <summary>
        /// Execute the command which returns multiple recordsets.
        /// </summary>
        /// <returns>Result with multiple recordsets</returns>
        Task<IMultiQueryResultAsync> MultiQuery(string sql, IDatabaseCommandParameters param = null,
            CommandType commandType = CommandType.StoredProcedure, int? commandTimeout = null);

        /// <summary>
        /// Execute the command which does not return recordsets.
        /// </summary>
        /// <returns>Number of rows affected</returns>
        Task<int> Execute(string sql, IDatabaseCommandParameters param = null,
            CommandType commandType = CommandType.StoredProcedure, int? commandTimeout = null);
    }
}