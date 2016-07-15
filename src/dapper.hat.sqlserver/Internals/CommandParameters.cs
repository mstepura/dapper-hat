using System.Collections.Generic;
using System.Data;

namespace Dapper.Hat.SqlServer
{
    sealed class CommandParameters : DynamicParameters, SqlMapper.IDynamicParameters, IDatabaseCommandParameters
    {
        private readonly List<NativeSqlServerParameter> _nativeParameters = new List<NativeSqlServerParameter>();

        private static readonly IReadOnlyDictionary<DbType, SqlDbType> _nativeParameterMap = new Dictionary<DbType, SqlDbType>
        {
            {DbType.Date, SqlDbType.Date},
            {DbType.Time, SqlDbType.Time},
        };

        public void Add<T>(string name, T value, DbType? dbType = null, ParameterDirection direction = ParameterDirection.Input, int? size = null)
        {
            SqlDbType customType;
            if (dbType.HasValue && _nativeParameterMap.TryGetValue(dbType.Value, out customType))
            {
                _nativeParameters.Add(
                    new NativeSqlServerParameter(
                        paramName: name,
                        dbType: customType,
                        direction: direction,
                        value: value,
                        size: size)
                    );
            }
            else
            {
                base.Add(name: name, value: value, dbType: dbType, size: size, direction: direction);
            }
        }

        public void AddOutput(string name, DbType dbType, int? size = null)
        {
            SqlDbType customType;
            if (_nativeParameterMap.TryGetValue(dbType, out customType))
            {
                _nativeParameters.Add(
                    new NativeSqlServerParameter(
                        paramName: name,
                        dbType: customType,
                        direction: ParameterDirection.Output,
                        size: size)
                    );
            }
            else
            {
                base.Add(name: name, dbType: dbType, size: size, direction: ParameterDirection.Output);
            }
        }

        public void AddReturnValue(string name = "@result")
        {
            base.Add(name: name, dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
        }

        public void AddTable<T>(string name, IEnumerable<T> tvp) where T : class
        {
            _nativeParameters.Add(TvpParameter.Create(name, tvp, null));
        }

        T IDatabaseCommandParameters.Get<T>(string name)
        {
            return base.Get<T>(name);
        }

        void SqlMapper.IDynamicParameters.AddParameters(IDbCommand command, SqlMapper.Identity identity)
        {
            base.AddParameters(command, identity);
            foreach (var param in _nativeParameters)
            {
                param.AddParameter(command);
            }
        }
    }
}