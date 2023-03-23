using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace Vostok.Commons.Helpers.Comparers
{
    [PublicAPI]
    internal class DictionaryComparer<TKey, TValue> : IEqualityComparer<IReadOnlyDictionary<TKey, TValue>>
    {
        public static readonly DictionaryComparer<TKey, TValue> Instance = new DictionaryComparer<TKey, TValue>();

        private readonly IEqualityComparer<TValue> valueComparer;

        public DictionaryComparer(IEqualityComparer<TValue> valueComparer = null)
        {
            this.valueComparer = valueComparer ?? EqualityComparer<TValue>.Default;
        }

        public bool Equals(IReadOnlyDictionary<TKey, TValue> x, IReadOnlyDictionary<TKey, TValue> y)
        {
            if (ReferenceEquals(x, y))
                return true;

            if (x == null || y == null)
                return false;

            if (x.Count != y.Count)
                return false;

            foreach (var pair in x)
            {
                if (!y.TryGetValue(pair.Key, out var yValue) || !valueComparer.Equals(pair.Value, yValue))
                    return false;
            }

            return true;
        }

        // ReSharper disable once ConditionIsAlwaysTrueOrFalse
        public int GetHashCode(IReadOnlyDictionary<TKey, TValue> dictionary)
            => dictionary == null ? 0 : dictionary.Aggregate(dictionary.Count, (current, element) => current ^ valueComparer.GetHashCode(element.Value));
    }
}