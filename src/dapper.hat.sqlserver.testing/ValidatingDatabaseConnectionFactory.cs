using System.Data.SqlClient;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Dapper.Hat.SqlServer.Testing
{
    /// <summary>
    /// Сonnection factory implementation for integration testing.
    /// </summary>
    public class ValidatingDatabaseConnectionFactory : IDatabaseConnectionFactory
    {
        private readonly string _connectionString;

        /// <summary>
        /// Construct instance of validating connection factory
        /// </summary>
        public ValidatingDatabaseConnectionFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Create sql connection used in interation tests.
        /// </summary>
        public Task<IDatabaseConnection> Create()
        {
            var sqlConnection = new SqlConnection(_connectionString);
            sqlConnection.InfoMessage += OnInfoMessage;

            return Task.FromResult<IDatabaseConnection>(new ValidatingDatabaseConnection(sqlConnection));
        }

        /// <summary>
        /// Create parameters container for commands or queries used in integration tests.
        /// </summary>
        public IDatabaseCommandParameters CreateParameters()
        {
            return new ValidatingCommandParameters(
                DatabaseConnectionFactory.CreateParameters()
                );
        }

        private static void OnInfoMessage(object sender, SqlInfoMessageEventArgs args)
        {
            Trace.TraceInformation(args.Message);
        }
    }
}
