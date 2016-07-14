using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Dapper.Hat.SqlServer.Testing
{
    sealed class ValidatingCommandParameters : IDatabaseCommandParameters, SqlMapper.IDynamicParameters
    {
        readonly IDatabaseCommandParameters _nestedParameters;
        List<SqlParameter> _interceptedParameters = null;

        public ValidatingCommandParameters(IDatabaseCommandParameters nestedParameters)
        {
            _nestedParameters = nestedParameters;
        }

        public IReadOnlyList<SqlParameter> InterceptedParameters
        {
            get { return _interceptedParameters; }
        }

        public void Add<T>(string name, T value, DbType? dbType = null, ParameterDirection direction = ParameterDirection.Input, int? size = null)
        {
            _nestedParameters.Add(name, value, dbType, direction, size);
        }

        public void AddOutput(string name, DbType dbType, int? size = null)
        {
            _nestedParameters.AddOutput(name, dbType, size);
        }

        public void AddReturnValue(string name = "@result")
        {
            _nestedParameters.AddReturnValue(name);
        }

        public void AddTable<T>(string name, IEnumerable<T> tvp) where T : class
        {
            _nestedParameters.AddTable(name, tvp);
        }

        public T Get<T>(string name)
        {
            // Additional check here.
            // Normally, Get will fail in test mode when when non-nullable parameter value is expected.
            // If T is a value type and is not nullable, make a call to Get<Nullable<T>> but return default(T)
            if (typeof(T).IsValueType && null == Nullable.GetUnderlyingType(typeof(T)))
            {
                var nullableType = typeof(Nullable<>).MakeGenericType(typeof(T));
                var getMethodForNullableType =
                    typeof(IDatabaseCommandParameters).GetMethod("Get").MakeGenericMethod(nullableType);

                getMethodForNullableType.Invoke(_nestedParameters, new object[] { name });

                return default(T);
            }
            return _nestedParameters.Get<T>(name);
        }

        void SqlMapper.IDynamicParameters.AddParameters(IDbCommand command, SqlMapper.Identity identity)
        {
            ((SqlMapper.IDynamicParameters)_nestedParameters).AddParameters(command, identity);

            _interceptedParameters = command.Parameters.OfType<SqlParameter>().ToList();
        }
    }
}
