using System;
using System.Collections.Generic;
using System.Linq;
using Vostok.Commons.Collections;
using Vostok.Commons.Formatting;

namespace Vostok.Logging.Abstractions
{
    internal static class LogPropertiesExtensions
    {
        public static ImmutableArrayDictionary<string, object> WithObjectProperties<T>(
            this ImmutableArrayDictionary<string, object> properties,
            T @object,
            bool allowOverwrite,
            bool allowNullValues)
        {
            if (@object == null)
                return properties;

            var pairs = @object is IReadOnlyDictionary<string, object> dictionary
                ? dictionary.Select(pair => (pair.Key, pair.Value))
                : ObjectPropertiesExtractor.ExtractProperties(@object);

            foreach (var (name, value) in pairs)
            {
                if (!allowNullValues && value == null)
                    continue;

                properties = properties.Set(name, value, allowOverwrite);
            }

            return properties;
        }

        public static ImmutableArrayDictionary<string, object> GenerateInitialObjectProperties<T>(
            T @object,
            bool allowNullValues)
        {
            if (@object == null)
                return null;

            if (@object is IReadOnlyDictionary<string, object> dictionary)
            {
                var properties = LogEvent.CreateProperties(dictionary.Count);
                foreach (var o in dictionary)
                {
                    if (!allowNullValues && o.Value == null)
                        continue;

                    //(deniaa): We don't know anything about keys comparer in this dictionary so we need to check keys uniqueness.
                    properties.SetUnsafe(o.Key, o.Value, true);
                }

                return properties;
            }

            //(deniaa): Object properties are always unique by design so we can fill immutable array dictionary without worrying about using the ImmutableArrayDictionary.Set method and overwrite flag.
            var (count, pairs) = ObjectPropertiesExtractor.ExtractPropertiesWithCount(@object);
            var emptyProperties = LogEvent.CreateProperties(Math.Max(4, count));
            foreach (var (key, value) in pairs)
            {
                if (!allowNullValues && value == null)
                    continue;

                emptyProperties.AppendUnsafe(key, value);
            }
            return emptyProperties;
        }
    }
}
