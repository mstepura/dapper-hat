using System;

namespace Dapper.Hat.SqlServer
{
    /// <summary>
    /// Explicit table type declaration for table valued parameters.
    /// Apply to the class which is mapped to SQL Server table type.
    /// <seealso cref="TvpPropertyDataTypeAttribute"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = true, AllowMultiple = false)]
    public sealed class TvpStructuredTypeNameAttribute : Attribute
    {
        private readonly string _typeName;

        /// <summary>
        /// Maps class to a specific SQL Server table type.
        /// </summary>
        /// <param name="typeName">SQL Server name of the table type, use two-component identifier.</param>
        public TvpStructuredTypeNameAttribute(string typeName)
        {
            if (string.IsNullOrWhiteSpace(typeName)) throw new ArgumentException("Type name should be non-empty string", nameof(typeName));
            if (typeName.Split('.').Length != 2)
                throw new ArgumentException($"Type name must be a valid 2-component Sql Server identifier", nameof(typeName));
            _typeName = typeName;
        }

        /// <summary>
        /// Table type name.
        /// </summary>
        public string TypeName
        {
            get { return _typeName; }
        }
    }
}
