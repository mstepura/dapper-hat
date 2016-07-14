using System;
using System.Data;
using System.Data.SqlClient;

namespace Dapper.Hat.SqlServer.Testing
{
    /// <summary>
    /// Maintains open connection with SET FMTONLY ON during object lifetime
    /// </summary>
    sealed class FmtOnlyConnectionContext : IDisposable
    {
        private readonly SqlConnection _connection;
        private readonly bool _closed;

        public FmtOnlyConnectionContext(SqlConnection connection)
        {
            _connection = connection;
            _closed = (connection.State == ConnectionState.Closed);

            if (_closed)
            {
                connection.Open();
            }

            connection.Execute("SET FMTONLY ON");
        }

        public void Dispose()
        {
            if (_connection.State == ConnectionState.Open)
            {
                if (_closed)
                {
                    _connection.Close();
                }
                else
                {
                    _connection.Execute("SET FMTONLY OFF");
                }
            }
        }
    }
}
