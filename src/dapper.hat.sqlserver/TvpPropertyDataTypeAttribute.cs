using System;
using System.Data;

namespace Dapper.Hat.SqlServer
{
    /// <summary>
    /// Explicit SQL field data type declaration for table valued parameters.
    /// Apply to every property of the class, which is mapped to SQL Server table type.
    /// <seealso cref="TvpStructuredTypeNameAttribute"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public sealed class TvpPropertyDataTypeAttribute : Attribute
    {
        readonly SqlDbType _dbType;
        readonly long? _maxLength;
        readonly byte? _precision;
        readonly byte? _scale;

        /// <summary>
        /// Maps property to a specific SQL Server data type.
        /// </summary>
        public TvpPropertyDataTypeAttribute(SqlDbType dbType)
        {
            _dbType = dbType;
        }

        /// <summary>
        /// Maps property to a specific SQL Server data type of variable length.
        /// </summary>
        public TvpPropertyDataTypeAttribute(SqlDbType dbType, long maxLength)
        {
            _dbType = dbType;
            _maxLength = maxLength;
        }

        /// <summary>
        /// Maps property to a specific SQL Server data type of specific precision and scale, e.g. DECIMAL
        /// </summary>
        public TvpPropertyDataTypeAttribute(SqlDbType dbType, byte precision, byte scale)
        {
            _dbType = dbType;
            _precision = precision;
            _scale = scale;
        }

        /// <summary>
        /// SQL Server native data type.
        /// </summary>
        public SqlDbType DbType { get { return _dbType; } }

        /// <summary>
        /// Maximum length for variable-length data types.
        /// </summary>
        public long? MaxLength { get { return _maxLength; } }

        /// <summary>
        /// Precision for fixed-point numeric types.
        /// </summary>
        public byte? Precision { get { return _precision; } }

        /// <summary>
        /// Scale for fixed-point numeric types.
        /// </summary>
        public byte? Scale { get { return _scale; } }
    }
}
