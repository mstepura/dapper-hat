using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Dapper.Hat.SqlServer.Testing
{
    /// <summary>
    /// Supporting class for database integration testing
    /// </summary>
    static class SqlQueryValidateExtension
    {
        /// <summary>
        /// Validation version of SqlConnection.Execute
        /// </summary>
        /// <remarks>
        /// 1. Run command with SET FMTONLY ON settings and intercept the parameter names and types which are created for SqlCommand object
        /// 2. Query parameters for the stored procedure and compare names and types to intercepted parameters
        /// </remarks>
        public static int ExecuteValidate(
            this SqlConnection connection,
            string commandText,
            IDatabaseCommandParameters parameters,
            CommandType commandType
            )
        {
            var validatingParameters = (ValidatingCommandParameters)parameters;

            var cmdDefinition = new CommandDefinition(
                commandText: commandText,
                parameters: parameters,
                commandType: commandType);

            // check integration with sql server
            // at this point ValidatingDatabaseCommandParameters will intercept command parameters
            using (new FmtOnlyConnectionContext(connection))
            {
                connection.Execute(cmdDefinition);
            }

            if (commandType == CommandType.StoredProcedure)
            {
                // for stored procedures perform advanced parameter check
                SqlQueryValidators.ValidateParametersForStoredProcedure(
                    validatingParameters,
                    connection,
                    commandText
                    );
            }

            return 0;
        }

        /// <summary>
        /// Validation version of SqlConnection.Query
        /// </summary>
        /// <remarks>
        /// 1. Run command with SET FMTONLY ON settings and intercept the parameter names and types which are created for SqlCommand object
        /// 2. Query parameters for the stored procedure and compare names and types to intercepted parameters
        /// 3. Check recordset with sys.dm_exec_describe_first_result_set function
        /// </remarks>
        public static IEnumerable<T> QueryValidate<T>(
            this SqlConnection connection,
            string queryText,
            IDatabaseCommandParameters parameters,
            CommandType commandType
            )
        {
            var cmdDefinition = new CommandDefinition(
                commandText: queryText,
                parameters: parameters,
                commandType: commandType);

            var validatingParameters = (ValidatingCommandParameters)parameters;
            if (commandType == CommandType.StoredProcedure)
            {
                // check integration with sql server
                // at this point ValidatingDatabaseCommandParameters will intercept command parameters
                using (new FmtOnlyConnectionContext(connection))
                {
                    connection.Execute(cmdDefinition);
                }

                // for stored procedures perform advanced parameter check
                SqlQueryValidators.ValidateParametersForStoredProcedure(
                    validatingParameters,
                    connection,
                    queryText
                    );
            }

            // do advanced recordset to class mapping check
            var recordsetStructure = DescribeFirstResultSet.GetRecordsetStructure(
                connection,
                queryText
                );

            SqlQueryValidators.ValidateRecordsetStructure(
                recordsetStructure,
                typeof(T),
                queryText
                );

            return Enumerable.Empty<T>();
        }



        /// <summary>
        /// Validation version of SqlConnection.QueryMultiple
        /// </summary>
        /// <remarks>
        /// 1. Run command with SET FMTONLY ON setting and intercept the parameter names and types which are created for SqlCommand object
        /// 2. Query parameters for the stored procedure and compare names and types to intercepted parameters
        /// 3. Check 1st recordset with sys.dm_exec_describe_first_result_set function
        /// 4. Run command with SET FMTONLY ON setting and compare every recordset structure with corresponding mapping type
        /// </remarks>
        public static IMultiQueryResult QueryMultipleValidate(
            this SqlConnection connection,
            string queryText,
            IDatabaseCommandParameters parameters,
            CommandType commandType
            )
        {
            // check integration with sql server and construct GridReader
            // at this point ValidatingDatabaseCommandParameters will intercept command parameters
            using (new FmtOnlyConnectionContext(connection))
            using (var gridReader = connection.QueryMultiple(
                sql: queryText,
                param: parameters,
                commandType: commandType))
            {
            }

            var validatingParameters = (ValidatingCommandParameters)parameters;
            if (commandType == CommandType.StoredProcedure)
            {
                // for stored procedures perform advanced parameter check
                SqlQueryValidators.ValidateParametersForStoredProcedure(
                    validatingParameters,
                    connection,
                    queryText
                    );
            }

            return new ValidatingMultiQueryResult(
                connection: connection,
                commandText: queryText,
                parameters: parameters,
                commandType: commandType);
        }
    }
}
