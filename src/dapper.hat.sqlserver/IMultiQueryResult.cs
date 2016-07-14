using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dapper.Hat.SqlServer
{
    /// <summary>
    /// Query result for multiple typed recordsets.
    /// </summary>
    public interface IMultiQueryResult : IDisposable
    {
        /// <summary>
        /// Reads next typed recordset.
        /// </summary>
        Task<IEnumerable<T>> Read<T>();
    }
}