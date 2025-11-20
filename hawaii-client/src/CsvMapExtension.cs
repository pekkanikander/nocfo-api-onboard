// CsvMapExtensions.cs
using System;
using System.Linq.Expressions;
using CsvHelper.Configuration;

namespace Nocfo.CsvHelpers {
    public static class CsvMapExtensions
    {
        public static MemberMap MapBoxed<T>(
        this DefaultClassMap<T> map,
        Expression<Func<T, object?>> expr,
        bool useExistingMap = true)
        => map.Map(expr, useExistingMap); // binds the non-generic overload
    }
}
