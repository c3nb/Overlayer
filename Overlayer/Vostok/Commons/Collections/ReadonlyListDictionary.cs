using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace Vostok.Commons.Collections
{
    /// <summary>
    /// An implementation of <see cref="IReadOnlyDictionary{TKey,TValue}"/> over <see cref="IReadOnlyList{T}"/> of <see cref="KeyValuePair{TKey,TValue}"/>
    /// </summary>
    [PublicAPI]
    internal class ReadonlyListDictionary<TKey, TValue> : IReadOnlyDictionary<TKey, TValue>
    {
        private readonly IReadOnlyList<KeyValuePair<TKey, TValue>> pairs;
        private readonly IEqualityComparer<TKey> keyComparer;

        public ReadonlyListDictionary([NotNull] IReadOnlyList<KeyValuePair<TKey, TValue>> pairs, [CanBeNull] IEqualityComparer<TKey> keyComparer = null)
        {
            this.pairs = pairs ?? throw new ArgumentNullException(nameof(pairs));
            this.keyComparer = keyComparer ?? EqualityComparer<TKey>.Default;
        }

        public int Count => pairs.Count;

        public IEnumerable<TKey> Keys
            => pairs.Select(pair => pair.Key);

        public IEnumerable<TValue> Values
            => pairs.Select(pair => pair.Value);

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
            => pairs.GetEnumerator();

        public bool ContainsKey(TKey key)
            => TryGetValue(key, out _);

        public bool TryGetValue(TKey key, out TValue value)
        {
            foreach (var pair in pairs)
            {
                if (keyComparer.Equals(pair.Key, key))
                {
                    value = pair.Value;
                    return true;
                }
            }

            value = default;
            return false;
        }

        public TValue this[TKey key]
            => TryGetValue(key, out var value) ? value : throw new KeyNotFoundException();

        IEnumerator IEnumerable.GetEnumerator() =>
            GetEnumerator();
    }
}