using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Reflection;

namespace Dapper.Hat.SqlServer.Testing
{
    /// <summary>
    /// Validators for recordset structure and parameters
    /// </summary>
    static class SqlQueryValidators
    {
        private static void Fail(string message, params object[] args)
        {
            throw new SqlTypeException(string.Format(message, args));
        }

        /// <summary>
        /// Validate recordset structure agains mapped type.
        /// </summary>
        /// <exception cref="System.Data.SqlTypes.SqlTypeException">Method throws exception if validation fails.</exception>
        /// <remarks>
        /// The following criteria are evaluated:
        /// - check if single field can be mapped to a type if mapped type is a primitive, nullable primitive, string, enum or nullable enum
        /// - check if each field can be mapped to corresponding property of a type
        /// - check if each type property has been mapped
        /// </remarks>
        /// <param name="sqlFields">Recordset fields and their types</param>
        /// <param name="type">Type to map recordset to</param>
        /// <param name="commandText">Stored procedure name or query text</param>
        public static void ValidateRecordsetStructure(
            IReadOnlyCollection<RecordsetFieldType> sqlFields,
            Type type,
            string commandText
            )
        {
            // check if mapped type is a primitive, nullable primitive, string, enum or nullable enum
            var mapToValueType = type.IsPrimitive
                || (type.IsValueType && Nullable.GetUnderlyingType(type) != null &&
                    (Nullable.GetUnderlyingType(type).IsPrimitive || Nullable.GetUnderlyingType(type).IsEnum)
                    )
                || type == typeof(string)
                || type.IsEnum;


            if (mapToValueType && sqlFields.Count != 1)
            {
                Fail("Cannot map record type with more than one field to {0}", type);
            }

            var actualFields = mapToValueType
                ? sqlFields.Take(1).Select(t => new { Type = type, Name = t.Name })
                : type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty)
                    .Select(property => new
                    {
                        Type = property.PropertyType,
                        Name = property.Name
                    });


            var fieldMap = actualFields.FullOuterJoin(
                sqlFields,
                actual => actual.Name,
                expected => expected.Name,
                (actual, expected) => new { actual, expected },
                StringComparer.OrdinalIgnoreCase
                ).ToList();

            // find what is missing in mapping type
            var missingFields = fieldMap
                .Where(pair => pair.actual == null)
                .Select(pair => pair.expected)
                .ToList();

            // find what is redundant in mapping type
            var redundantFields = fieldMap
                .Where(pair => pair.expected == null)
                .Select(pair => pair.actual)
                .ToList();

            // find type mismatch
            var comparer = new FieldTypeCompatibilityComparer();
            var unmatchedFields = fieldMap
                .Where(pair => pair.expected != null && pair.actual != null
                               && !comparer.CanMapTo(pair.expected.Type, pair.actual.Type)
                ).ToList();

            // if anything wrong found - generate failure
            var exceptionText = string.Empty;
            if (missingFields.Any())
            {
                exceptionText += string.Join("",
                    missingFields.Select(prm =>
                        string.Format(
                            "\nMissing property {0} of type {1}",
                            prm.Name,
                            prm.Type)
                        )
                    );
            }

            if (redundantFields.Any())
            {
                exceptionText += string.Join("",
                    redundantFields.Select(prm =>
                        string.Format(
                            "\nRedundant property {0} of type {1}",
                            prm.Name,
                            prm.Type
                            )
                        )
                    );
            }

            if (unmatchedFields.Any())
            {
                exceptionText += string.Join("",
                    unmatchedFields.Select(prm =>
                        string.Format(
                            "\nProperty {0} type mismatch: found type {1}, expected type {2}",
                            prm.actual.Name,
                            prm.actual.Type,
                            prm.expected.Type
                            )
                        )
                    );
            }

            if (!string.IsNullOrWhiteSpace(exceptionText))
            {
                Fail("{0} dataset validation failed:\n{1}", commandText, exceptionText);
            }
        }


        /// <summary>
        /// Check if parameters processed by Dapper command are the same as expected by the stored procedure.
        /// </summary>
        /// <remarks>
        /// Optional parameters are treated as required by this method.
        /// </remarks>
        /// <param name="validatingParameters">Parameters intercepted when Dapped executes the command</param>
        /// <param name="connection">Sql connection object</param>
        /// <param name="commandText">Stored procedure name</param>
        public static void ValidateParametersForStoredProcedure(
            ValidatingCommandParameters validatingParameters,
            SqlConnection connection,
            string commandText
            )
        {
            // advanced parameter check
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = commandText;

                var connectionWasInClosedState = (connection.State == ConnectionState.Closed);
                if (connectionWasInClosedState)
                    connection.Open();
                try
                {
                    SqlCommandBuilder.DeriveParameters(cmd);
                }
                finally
                {
                    if (connectionWasInClosedState && connection.State == ConnectionState.Open)
                        connection.Close();
                }

                // in both actual and expected parameter collections
                // exclude ReturnValue parameters as their names may be different and this is acceptable
                var expectedParameters = cmd.Parameters
                    .OfType<SqlParameter>()
                    .Where(prm => prm.Direction != ParameterDirection.ReturnValue)
                    .ToList();

                var actualParameters = (
                    validatingParameters != null && validatingParameters.InterceptedParameters != null
                        ? validatingParameters.InterceptedParameters
                            .Where(prm => prm.Direction != ParameterDirection.ReturnValue)
                        : Enumerable.Empty<SqlParameter>()
                    ).ToList();

                ValidateParameterCollections(expectedParameters, actualParameters, commandText);
            }
        }


        /// <summary>
        /// Compare parameter collections
        /// </summary>
        private static void ValidateParameterCollections(
            IReadOnlyList<SqlParameter> expectedParameters,
            IReadOnlyList<SqlParameter> actualParameters,
            string commandText
            )
        {
            // now check parameters collection if they match
            var parameterMap = expectedParameters.FullOuterJoin(
                actualParameters,
                expected => expected.ParameterName.TrimStart('@'),
                actual => actual.ParameterName.TrimStart('@'),
                (expected, actual) => new { expected, actual },
                StringComparer.OrdinalIgnoreCase
                ).ToList();

            // find parameters missing in call from the data layer
            var missingParameters = parameterMap
                .Where(pair => pair.actual == null)
                .Select(pair => pair.expected)
                .ToList();

            // find parameters which are redundant
            var redundantParameters = parameterMap
                .Where(pair => pair.expected == null)
                .Select(pair => pair.actual)
                .ToList();

            // find parameters with differences in metadata
            var comparer = new SqlParameterMetadataEqualityComparer();
            var unmatchedParameters = parameterMap
                .Where(pair => pair.expected != null && pair.actual != null
                               && !comparer.Equals(pair.expected, pair.actual)
                )
                .ToList();

            // if anything wrong found - generate failure
            var exceptionText = string.Empty;
            if (missingParameters.Any())
            {
                exceptionText += string.Join("",
                    missingParameters.Select(prm =>
                        string.Format(
                            "\nMissing parameter {0} of type {1}{2}",
                            prm.ParameterName,
                            prm.SqlDbType,
                            prm.Size != 0 ? string.Format(" size {0}", prm.Size) : string.Empty
                            )
                        )
                    );
            }

            if (redundantParameters.Any())
            {
                exceptionText += string.Join("",
                    redundantParameters.Select(prm =>
                        string.Format(
                            "\nRedundant parameter {0} of type {1}{2}",
                            prm.ParameterName,
                            prm.SqlDbType,
                            prm.Size != 0 ? string.Format(" size {0}", prm.Size) : string.Empty
                            )
                        )
                    );
            }

            if (unmatchedParameters.Any())
            {
                exceptionText += string.Join("",
                    unmatchedParameters.Select(prm =>
                        string.Format(
                            "\nParameter mismatch {0}: found type {1} size {2}, expected type {3} size {4}",
                            prm.actual.ParameterName,
                            prm.actual.SqlDbType,
                            prm.actual.Size,
                            prm.expected.SqlDbType,
                            prm.expected.Size
                            )
                        )
                    );
            }

            if (!string.IsNullOrWhiteSpace(exceptionText))
            {
                Fail("Stored procedure {0} parameters validation failed:\n{1}", commandText, exceptionText);
            }
        }
    }
}
