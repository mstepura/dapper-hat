using System;

namespace Dapper.Hat.SqlServer.Testing
{
    /// <summary>
    /// Represents one column of the recordset 
    /// </summary>
    sealed class RecordsetFieldType
    {
        public string Name { get; set; }
        public Type Type { get; set; }
    }
}
