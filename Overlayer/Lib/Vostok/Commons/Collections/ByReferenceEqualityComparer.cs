using System.Collections.Generic;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace Vostok.Commons.Collections
{
    [PublicAPI]
    internal class ByReferenceEqualityComparer<T> : IEqualityComparer<T>
    {
        public static readonly ByReferenceEqualityComparer<T> Instance = new ByReferenceEqualityComparer<T>();

        public bool Equals(T x, T y) => ReferenceEquals(x, y);

        public int GetHashCode(T obj) => RuntimeHelpers.GetHashCode(obj);
    }
}