using System;
using System.Data;
using System.Data.SqlClient;

namespace Dapper.Hat.SqlServer
{
    sealed class NativeSqlServerParameter
    {
        readonly string _paramName;
        readonly SqlDbType _dbType;
        readonly object _value;
        readonly ParameterDirection _direction;
        readonly int? _size;
        readonly byte? _scale;
        readonly byte? _precision;
        readonly string _typeName;

        public NativeSqlServerParameter(
            string paramName,
            SqlDbType dbType,
            ParameterDirection direction,
            object value = null,
            int? size = null,
            byte? scale = null,
            byte? precision = null,
            string typeName = null)
        {
            _paramName = paramName;
            _dbType = dbType;
            _value = value;
            _direction = direction;
            _size = size;
            _scale = scale;
            _precision = precision;
            _typeName = typeName;
        }

        public void AddParameter(
            IDbCommand command
            )
        {
            var parameter = (SqlParameter)command.CreateParameter();
            parameter.SqlDbType = _dbType;
            parameter.ParameterName = _paramName;
            parameter.Value = _value ?? DBNull.Value;
            parameter.Direction = _direction;
            if (!string.IsNullOrEmpty(_typeName)) parameter.TypeName = _typeName;
            if (_size.HasValue) parameter.Size = _size.Value;
            if (_scale.HasValue) parameter.Scale = _scale.Value;
            if (_precision.HasValue) parameter.Precision = _precision.Value;
            command.Parameters.Add(parameter);
        }
    }
}
