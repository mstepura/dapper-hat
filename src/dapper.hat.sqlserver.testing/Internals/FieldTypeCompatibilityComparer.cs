using System;

namespace Dapper.Hat.SqlServer.Testing
{
    /// <summary>
    /// Field type comparer.
    /// This class maintains criteria of field compatibility.
    /// </summary>
    sealed class FieldTypeCompatibilityComparer
    {
        public bool CanMapTo(Type recordsetType, Type mappedType)
        {
            if (mappedType.IsEnum)
            {
                return (mappedType.GetEnumUnderlyingType() == recordsetType);
            }

            var underlyingType = mappedType.IsValueType
                ? Nullable.GetUnderlyingType(mappedType)
                : null;

            if (underlyingType != null)
            {
                if (underlyingType.IsEnum)
                {
                    return (underlyingType.GetEnumUnderlyingType() == recordsetType);
                }

                return (underlyingType.UnderlyingSystemType == recordsetType);
            }

            // exceptions: allow char/string type conversion
            if (mappedType == typeof(char) && recordsetType == typeof(string))
                return true;

            return (mappedType == recordsetType);
        }
    }
}
