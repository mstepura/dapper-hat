using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Threading.Tasks;

namespace Dapper.Hat.SqlServer.Testing
{
    sealed class ValidatingMultiQueryResult : IMultiQueryResult
    {
        private readonly SqlConnection _connection;
        private readonly string _commandText;
        private readonly IDatabaseCommandParameters _parameters;
        private readonly CommandType _commandType;

        private bool _nextResultAvailable = true;
        private int _resultNumber = 0;
        private IDataReader _dataReader = null;

        public ValidatingMultiQueryResult(
            SqlConnection connection,
            string commandText,
            IDatabaseCommandParameters parameters,
            CommandType commandType
            )
        {
            _connection = connection;
            _commandText = commandText;
            _parameters = parameters;
            _commandType = commandType;
        }

        public Task<IEnumerable<T>> Read<T>()
        {
            if (!_nextResultAvailable)
            {
                throw new SqlTypeException(
                    string.Format("ReadMultiple {0} - recordset [{1}] or expected type {2} not available",
                        _commandText,
                        _resultNumber,
                        typeof(T)
                        )
                    );
            }

            // Reset flag to false and set it back after ValidateRecordsetStructure.
            // In case ValidateRecordsetStructure throws exception
            // and Dispose is called, we want to suppress 'more recordsets available' exeption.
            _nextResultAvailable = false;

            if (_resultNumber == 0)
            {
                // extensive check of the first resultset
                var firstRecordsetStructure = DescribeFirstResultSet.GetRecordsetStructure(
                    _connection,
                    _commandText
                    );

                SqlQueryValidators.ValidateRecordsetStructure(
                    firstRecordsetStructure,
                    typeof(T),
                    _commandText
                    );


                // Create reader in FMT_ONLY mode
                // which will retrieve 1st and all subsequent recordset structures
                var cmdDefinition = new CommandDefinition(
                    commandText: _commandText,
                    parameters: _parameters,
                    commandType: _commandType);
                _dataReader = _connection.ExecuteReader(cmdDefinition, CommandBehavior.SchemaOnly);
            }

            // check result structure here
            var currentResultStructure = Enumerable.Range(0, _dataReader.FieldCount).Select(
                idx => new RecordsetFieldType
                {
                    Name = _dataReader.GetName(idx),
                    Type = _dataReader.GetFieldType(idx)
                }).ToList();

            SqlQueryValidators.ValidateRecordsetStructure(
                currentResultStructure,
                typeof(T),
                _commandText
                );

            _resultNumber++;
            _nextResultAvailable = _dataReader.NextResult();

            return Task.FromResult(Enumerable.Empty<T>());
        }

        public void Dispose()
        {
            if (_dataReader != null)
            {
                _dataReader.Dispose();
            }

            if (_nextResultAvailable)
            {
                throw new SqlTypeException(string.Format("ReadMultiple {0} - more recordsets available", _commandText));
            }
        }
    }
}
