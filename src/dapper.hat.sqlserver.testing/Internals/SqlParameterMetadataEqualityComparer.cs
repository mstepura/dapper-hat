using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Dapper.Hat.SqlServer.Testing
{
    sealed class SqlParameterMetadataEqualityComparer : IEqualityComparer<SqlParameter>
    {
        public bool Equals(SqlParameter x, SqlParameter y)
        {
            if (x == null && y == null)
                return true;
            if (x == null || y == null)
                return false;
            if (x.SqlDbType != y.SqlDbType)
                return false;
            if (x.Size != y.Size)
                return false;

            // special case, output parameters are Input/Output from SQL Server point of view
            var directionX = x.Direction == ParameterDirection.Output
                ? ParameterDirection.InputOutput
                : x.Direction;
            var directionY = y.Direction == ParameterDirection.Output
                ? ParameterDirection.InputOutput
                : y.Direction;

            if (directionX != directionY)
                return false;

            return true;
        }

        public int GetHashCode(SqlParameter parameter)
        {
            var direction = parameter.Direction == ParameterDirection.Output
                ? ParameterDirection.InputOutput
                : parameter.Direction;

            return parameter.SqlDbType.GetHashCode()
                   ^ (parameter.Size.GetHashCode() << 2)
                   ^ (direction.GetHashCode() << 4);
        }
    }
}
