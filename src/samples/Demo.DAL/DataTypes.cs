using System;
using System.Collections.Generic;

namespace Demo.DAL
{
    public enum ShortEnumerableType : short
    {
        None = 0, Value1 = 1, Value2 = 2
    }

    public enum LongEnumerableType : long
    {
        None = 0, Value1 = 1, Value2 = 2
    }

    public interface IComplexType
    {
        ShortEnumerableType Field1 { get; }
        LongEnumerableType? Field2 { get; }
        DateTime Field3 { get; }
        string Field4 { get; }
        bool FieldBoolean { get; }
    }

    public interface IManyDatasetResult
    {
        IEnumerable<int> Result1 { get; }
        IEnumerable<string> Result2 { get; }
    }
}
