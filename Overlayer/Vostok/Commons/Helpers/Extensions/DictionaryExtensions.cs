using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Vostok.Commons.Helpers.Extensions
{
    [PublicAPI]
    internal static class DictionaryExtensions
    {
        [Obsolete("Use ?.GetValueOrDefault() instead.")]
        public static TValue GetValueOrNull<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary, TKey key)
            where TValue : class =>
            dictionary != null && dictionary.TryGetValue(key, out var value) ? value : null;

        public static Dictionary<TKey, TValue> With<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue newValue)
        {
            dictionary[key] = newValue;
            return dictionary;
        }

        public static IReadOnlyDictionary<TKey, TValue> With<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary, TKey key, TValue newValue) =>
            dictionary.ToDictionary().With(key, newValue);

        public static Dictionary<TKey, TValue> Without<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key)
        {
            dictionary.Remove(key);
            return dictionary;
        }

        public static IReadOnlyDictionary<TKey, TValue> Without<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary, TKey key) =>
            dictionary.ToDictionary().Without(key);

        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary) =>
            dictionary.Select(p => p).ToDictionary(p => p.Key, p => p.Value);

        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> enumerable) =>
            enumerable.ToDictionary(p => p.Key, p => p.Value);

        public static async Task<Dictionary<TKey, TValue>> ToDictionaryAsync<TSource, TKey, TValue>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            Func<TSource, Task<TValue>> valueSelector)
        {
            var result = new Dictionary<TKey, TValue>();

            foreach (var element in source)
                result[keySelector(element)] = await valueSelector(element).ConfigureAwait(false);

            return result;
        }
    }
}