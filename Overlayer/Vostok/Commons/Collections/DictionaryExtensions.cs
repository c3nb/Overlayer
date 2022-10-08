using System;
using System.Collections.Generic;

namespace Vostok.Commons.Collections
{
    internal static class DictionaryExtensions
    {
        public static TValue GetOrAdd<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, Func<TKey, TValue> factory)
        {
            if (dict.TryGetValue(key, out var value))
                return value;
            return dict[key] = factory(key);
        }
    }
}