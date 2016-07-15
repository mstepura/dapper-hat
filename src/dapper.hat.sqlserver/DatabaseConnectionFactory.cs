using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Dapper.Hat.SqlServer
{
    /// <summary>
    /// Database connection factory.
    /// </summary>
    public class DatabaseConnectionFactory : IDatabaseConnectionFactory
    {
        private readonly string _connectionString;
        private readonly int? _defaultCommandTimeout;

        /// <summary>
        /// Constructs connection factory.
        /// </summary>
        /// <param name="connectionString">ADO.NET connection string</param>
        /// <param name="defaultCommandTimeout">
        /// Default command timeout in seconds. If null, default command timeout of 30 seconds is used, <see cref="System.Data.SqlClient.SqlCommand.CommandTimeout"/>.
        /// Comand timeout can be overridden for individual command.
        /// </param>
        public DatabaseConnectionFactory(string connectionString, int? defaultCommandTimeout = null)
        {
            _connectionString = connectionString;
            _defaultCommandTimeout = defaultCommandTimeout;
        }

        /// <summary>
        /// Create database connection.
        /// </summary>
        /// <returns>Task with database connection result.</returns>
        public async Task<IDatabaseConnection> Create()
        {
            var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync().ConfigureAwait(false);
            return new DatabaseConnection(connection, _defaultCommandTimeout);
        }

        IDatabaseCommandParameters IDatabaseConnectionFactory.CreateParameters()
        {
            return DatabaseConnectionFactory.CreateParameters();
        }

        /// <summary>
        /// Create parameters collection.
        /// </summary>
        public static IDatabaseCommandParameters CreateParameters()
        {
            return new CommandParameters();
        }
    }
}