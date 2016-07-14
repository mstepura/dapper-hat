using System.Threading.Tasks;

namespace Dapper.Hat.SqlServer
{
    /// <summary>
    /// Database connection factory.
    /// </summary>
    public interface IDatabaseConnectionFactory
    {
        /// <summary>
        /// Creates database connection.
        /// </summary>
        Task<IDatabaseConnection> Create();

        /// <summary>
        /// Create parameter container for database operations.
        /// </summary>
        IDatabaseCommandParameters CreateParameters();
    }
}