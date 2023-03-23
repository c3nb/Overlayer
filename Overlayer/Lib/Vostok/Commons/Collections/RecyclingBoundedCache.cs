using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Vostok.Commons.Collections
{
    [PublicAPI]
    internal class RecyclingBoundedCache<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
    {
        private readonly int capacity;
        private readonly IEqualityComparer<TKey> comparer;
        private RecyclingBoundedCacheState state;

        public RecyclingBoundedCache(int capacity, [CanBeNull] IEqualityComparer<TKey> comparer = null)
        {
            if (capacity < 0)
                throw new ArgumentOutOfRangeException(nameof(capacity), "The capacity must be non-negative");

            this.capacity = capacity;
            this.comparer = comparer ?? EqualityComparer<TKey>.Default;

            state = new RecyclingBoundedCacheState(this.comparer);
        }

        public TValue Obtain(TKey key, Func<TKey, TValue> factory)
        {
            var currentState = state;
            if (currentState.Items.TryGetValue(key, out var value))
                return value;

            value = factory(key);
            TryAddToCachedItems(key, currentState, value);

            return value;
        }

        public TValue Obtain<TArg>(TKey key, TArg arg, Func<TKey, TArg, TValue> factory)
        {
            var currentState = state;
            if (currentState.Items.TryGetValue(key, out var value))
                return value;

            value = factory(key, arg);
            TryAddToCachedItems(key, currentState, value);

            return value;
        }

        public async Task<TValue> ObtainAsync(TKey key, Func<TKey, Task<TValue>> factory)
        {
            var currentState = state;
            if (currentState.Items.TryGetValue(key, out var value))
                return value;

            value = await factory(key).ConfigureAwait(false);
            TryAddToCachedItems(key, currentState, value);

            return value;
        }

        public bool TryRemove(TKey key)
        {
            var currentState = state;

            if (!currentState.Items.TryRemove(key, out _))
                return false;

            Interlocked.Decrement(ref currentState.Count);
            return true;
        }

        public bool TryGetValue(TKey key, out TValue value)
            => state.Items.TryGetValue(key, out value);

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
            => state.Items.GetEnumerator();

        private void TryAddToCachedItems(TKey key, RecyclingBoundedCacheState currentState, TValue value)
        {
            if (currentState.Items.TryAdd(key, value))
            {
                var newCount = Interlocked.Increment(ref currentState.Count);
                if (newCount > capacity)
                    Interlocked.Exchange(ref state, new RecyclingBoundedCacheState(comparer));
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private class RecyclingBoundedCacheState
        {
            public readonly ConcurrentDictionary<TKey, TValue> Items;

            public int Count;

            public RecyclingBoundedCacheState(IEqualityComparer<TKey> comparer)
            {
                Items = new ConcurrentDictionary<TKey, TValue>(comparer);
            }
        }
    }
}