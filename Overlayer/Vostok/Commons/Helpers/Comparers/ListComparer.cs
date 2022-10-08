using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace Vostok.Commons.Helpers.Comparers
{
    [PublicAPI]
    internal class ListComparer<T> : IEqualityComparer<IReadOnlyList<T>>
    {
        public static readonly ListComparer<T> Instance = new ListComparer<T>();

        private readonly IEqualityComparer<T> elementComparer;

        public ListComparer(IEqualityComparer<T> elementComparer = null)
        {
            this.elementComparer = elementComparer ?? EqualityComparer<T>.Default;
        }

        public bool Equals([ItemCanBeNull] IReadOnlyList<T> x, [ItemCanBeNull] IReadOnlyList<T> y)
        {
            if (ReferenceEquals(x, y))
                return true;

            if (x == null || y == null)
                return false;

            if (x.Count != y.Count)
                return false;

            for (var i = 0; i < x.Count; i++)
            {
                if (!elementComparer.Equals(x[i], y[i]))
                    return false;
            }

            return true;
        }

        // ReSharper disable once ConditionIsAlwaysTrueOrFalse
        public int GetHashCode([CanBeNull] [ItemCanBeNull] IReadOnlyList<T> list)
            => list == null ? 0 : list.Aggregate(list.Count, (current, element) => (current * 397) ^ elementComparer.GetHashCode(element));
    }
}