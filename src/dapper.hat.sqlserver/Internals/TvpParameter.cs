using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.SqlServer.Server;
using System.Linq;
using System.Reflection;

namespace Dapper.Hat.SqlServer
{
    static class TvpParameter
    {
        public static NativeSqlServerParameter Create<T>(string paramName, IEnumerable<T> table, string typeName)
        {
            var records = table.Select(data =>
            {
                var rec = new SqlDataRecord(TVPMapping<T>.Metadata);
                var idx = 0;
                foreach (var prop in TVPMapping<T>.Properties)
                {
                    rec.SetValue(idx, prop.GetValue(data, null));
                    idx++;
                }
                return rec;
            }).ToList();

            return new NativeSqlServerParameter(
                paramName: paramName,
                dbType: SqlDbType.Structured,
                direction: ParameterDirection.Input,
                typeName: TVPMapping<T>.NativeTypeName ?? typeName,
                value: records
                );
        }

        static readonly IReadOnlyDictionary<Type, SqlDbType> TypeMap = new Dictionary<Type, SqlDbType>
            {
            {typeof(byte), SqlDbType.TinyInt},
            {typeof(sbyte), SqlDbType.TinyInt},
            {typeof(short), SqlDbType.SmallInt},
            {typeof(ushort), SqlDbType.SmallInt},
            {typeof(int), SqlDbType.Int},
            {typeof(uint), SqlDbType.Int},
            {typeof(long), SqlDbType.BigInt},
            {typeof(ulong), SqlDbType.BigInt},
            {typeof(float), SqlDbType.Real},
            {typeof(double), SqlDbType.Float},
            {typeof(decimal), SqlDbType.Decimal},
            {typeof(bool), SqlDbType.Bit},
            {typeof(string), SqlDbType.NVarChar},
            {typeof(char), SqlDbType.NChar},
            {typeof(Guid), SqlDbType.UniqueIdentifier},
            {typeof(DateTime), SqlDbType.DateTime},
            {typeof(DateTimeOffset), SqlDbType.DateTimeOffset},
            {typeof(TimeSpan), SqlDbType.Time},
            {typeof(byte[]), SqlDbType.Binary},
            {typeof(byte?), SqlDbType.TinyInt},
            {typeof(sbyte?), SqlDbType.TinyInt},
            {typeof(short?), SqlDbType.SmallInt},
            {typeof(ushort?), SqlDbType.SmallInt},
            {typeof(int?), SqlDbType.Int},
            {typeof(uint?), SqlDbType.Int},
            {typeof(long?), SqlDbType.BigInt},
            {typeof(ulong?), SqlDbType.BigInt},
            {typeof(float?), SqlDbType.Real},
            {typeof(double?), SqlDbType.Float},
            {typeof(decimal?), SqlDbType.Decimal},
            {typeof(bool?), SqlDbType.Bit},
            {typeof(char?), SqlDbType.NChar},
            {typeof(Guid?), SqlDbType.UniqueIdentifier},
            {typeof(DateTime?), SqlDbType.DateTime},
            {typeof(DateTimeOffset?), SqlDbType.DateTimeOffset},
            {typeof(TimeSpan?), SqlDbType.Time},
            {typeof(Object), SqlDbType.Variant},
        };

        static class TVPMapping<T>
        {
            public static readonly SqlMetaData[] Metadata;
            public static readonly string NativeTypeName;
            public static readonly PropertyInfo[] Properties;

            static TVPMapping()
            {
                var typeAttr = typeof(T).GetCustomAttributes(typeof(TvpStructuredTypeNameAttribute), true).FirstOrDefault() as TvpStructuredTypeNameAttribute;

                NativeTypeName = typeAttr != null ? typeAttr.TypeName : null;

                Properties = typeof(T).GetProperties();

                Metadata = Properties.Select(f =>
                {
                    var attr = f.GetCustomAttributes(typeof(TvpPropertyDataTypeAttribute), true).FirstOrDefault() as TvpPropertyDataTypeAttribute;
                    if (attr == null)
                    {
                        SqlDbType dbType;
                        if (!TypeMap.TryGetValue(f.PropertyType, out dbType))
                            throw new NotSupportedException(string.Format("The member {0} of type {1} cannot be used as a parameter value", f.Name, f.PropertyType));
                        return new SqlMetaData(f.Name, dbType);
                    }

                    if (attr.MaxLength.HasValue)
                        return new SqlMetaData(f.Name, attr.DbType, attr.MaxLength.Value);
                    if (attr.Precision.HasValue)
                        return new SqlMetaData(f.Name, attr.DbType, attr.Precision.Value, attr.Scale ?? 0);
                    return new SqlMetaData(f.Name, attr.DbType);

                }).ToArray();
            }
        }
    }
}
