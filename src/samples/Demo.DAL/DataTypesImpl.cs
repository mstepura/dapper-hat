using System;
using System.Collections.Generic;
using System.Data;
using Dapper.Hat.SqlServer;

namespace Demo.DAL
{
    sealed class InternalComplexType : IComplexType
    {
        public ShortEnumerableType Field1 { get; private set; }
        public LongEnumerableType? Field2 { get; private set; }
        public DateTime Field3 { get; private set; }
        public string Field4 { get; private set; }
        public bool FieldBoolean { get; private set; }
    }

    sealed class InternalManyDatasetResult : IManyDatasetResult
    {
        public IEnumerable<int> Result1 { get; set; }
        public IEnumerable<string> Result2 { get; set; }
    }

    [TvpStructuredTypeName("[dbo].[t_IntegerList]")]
    sealed class InternalIntegerList
    {
        [TvpPropertyDataType(SqlDbType.Int)]
        public int? Id { get; set; }
    }
}
