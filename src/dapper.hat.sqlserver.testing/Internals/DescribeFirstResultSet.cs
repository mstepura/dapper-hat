using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;

namespace Dapper.Hat.SqlServer.Testing
{
    static class DescribeFirstResultSet
    {
        /// <summary>
        /// Return resultset structure of the query or stored proc.
        /// This function is supported by SQL2012+
        /// </summary>
        public static IReadOnlyCollection<RecordsetFieldType> GetRecordsetStructure(
            SqlConnection connection,
            string queryText
            )
        {
            // check 1st resultset with dm_exec_describe_first_result_set
            const string recordsetCheckingQuery = @"
select
    name,
    is_nullable,
    system_type_id,
    system_type_name,
    TYPE_NAME(system_type_id) as type_code,
    max_length,
    error_number,
    error_message
from sys.dm_exec_describe_first_result_set(@tsql, default, default)
";
            var recordsetCheckingParams = DatabaseConnectionFactory.CreateParameters();
            recordsetCheckingParams.Add(name: "@tsql", value: queryText, dbType: DbType.String, size: -1);
            var recordsetFields = connection.Query<RecordsetFieldSqlDescription>(
                sql: recordsetCheckingQuery,
                param: recordsetCheckingParams,
                commandType: CommandType.Text).ToList();

            // if any error is returned - stop processing and blow up immediately
            if (recordsetFields.Any(field => field.error_number.HasValue))
            {
                var errorText = string.Join("\n",
                    recordsetFields
                        .Where(field => field.error_number.HasValue)
                        .Select(field => string.Format("{0} {1}", field.error_number, field.error_message))
                    );
                throw new SqlTypeException(errorText);
            }

            return recordsetFields.Select(field =>
            {
                Type type;
                if (!SqlTypeMapping.TryGetValue(field.type_code, out type))
                    type = typeof(object);

                return new RecordsetFieldType { Name = field.name, Type = type };
            }).ToList();
        }


        /// <summary>
        /// Subset of information returned by [sp_describe_first_result_set] stored procedure
        /// or sys.dm_exec_describe_first_result_set function.
        /// <see href="https://msdn.microsoft.com/en-us/library/ff878602.aspx" />
        /// </summary>
        sealed class RecordsetFieldSqlDescription
        {
            // ReSharper disable InconsistentNaming
            public string name { get; set; }
            public bool is_nullable { get; set; }
            public int system_type_id { get; set; }
            public string system_type_name { get; set; }
            public string type_code { get; set; }
            public short max_length { get; set; }
            public int? error_number { get; set; }
            public string error_message { get; set; }
            // ReSharper restore InconsistentNaming
        }

        /// <summary>
        /// Type mapping from native sql TYPE_NAME(system_type_id) to standard .NET types
        /// </summary>
        static readonly IReadOnlyDictionary<string, Type> SqlTypeMapping =
            new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase)
        {
            {"bigint",typeof(long)},
            {"binary",typeof(byte[])},
            {"bit",typeof(bool)},
            {"char",typeof(string)},
            {"date",typeof(DateTime)},
            {"datetime",typeof(DateTime)},
            {"datetime2",typeof(DateTime)},
            {"datetimeoffset",typeof(DateTimeOffset)},
            {"decimal",typeof(decimal)},
            {"float",typeof(double)},
            {"image",typeof(byte[])},
            {"int",typeof(int)},
            {"money",typeof(decimal)},
            {"nchar",typeof(string)},
            {"ntext",typeof(string)},
            {"numeric",typeof(decimal)},
            {"nvarchar",typeof(string)},
            {"real",typeof(float)},
            {"smalldatetime",typeof(DateTime)},
            {"smallint",typeof(short)},
            {"smallmoney",typeof(decimal)},
            {"sql_variant",typeof(object)},
            {"text",typeof(string)},
            {"time",typeof(TimeSpan)},
            {"timestamp",typeof(byte[])},
            {"tinyint",typeof(byte)},
            {"uniqueidentifier",typeof(Guid)},
            {"varbinary",typeof(byte[])},
            {"varchar",typeof(string)},
            {"xml",typeof(string)},
        };
    }
}
