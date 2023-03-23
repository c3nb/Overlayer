using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using JetBrains.Annotations;
using Vostok.Commons.Collections;

// ReSharper disable ConvertClosureToMethodGroup

namespace Vostok.Commons.Formatting
{
    [PublicAPI]
    internal static class DictionaryInspector
    {
        private const int CacheCapacity = 1000;

        private static readonly RecyclingBoundedCache<Type, DictionaryInfo> Cache =
            new RecyclingBoundedCache<Type, DictionaryInfo>(CacheCapacity);

        public static bool IsSimpleDictionary(Type type) =>
            Cache.Obtain(type, t => TryDetectSimpleDictionary(t)).IsDictionary;

        public static IEnumerable<(string, object)> EnumerateSimpleDictionary(object dictionary)
        {
            var info = Cache.Obtain(dictionary.GetType(), t => TryDetectSimpleDictionary(t));

            foreach (var pair in (IEnumerable)dictionary)
                if (pair?.GetType() == info.PairType)
                    yield return (info.KeyGetter(pair).ToString(), info.ValueGetter(pair));
        }

        private static DictionaryInfo TryDetectSimpleDictionary(Type type)
        {
            try
            {
                var dictionaryInterface = type
                    .GetInterfaces()
                    .Where(iface => iface.IsGenericType)
                    .Where(iface => iface.GetGenericTypeDefinition() == typeof(IReadOnlyDictionary<,>))
                    .FirstOrDefault(iface => IsSimpleKeyType(iface.GenericTypeArguments[0]));

                if (dictionaryInterface == null)
                    return default;

                var result = new DictionaryInfo();

                var keyType = dictionaryInterface.GenericTypeArguments[0];
                var valueType = dictionaryInterface.GenericTypeArguments[1];
                var pairType = result.PairType = typeof(KeyValuePair<,>).MakeGenericType(keyType, valueType);

                var parameter = Expression.Parameter(typeof(object));
                var pair = Expression.Convert(parameter, pairType);

                var keyProperty = Expression.Convert(Expression.Property(pair, "Key"), typeof(object));
                var valueProperty = Expression.Convert(Expression.Property(pair, "Value"), typeof(object));

                result.KeyGetter = Expression.Lambda<Func<object, object>>(keyProperty, parameter).Compile();
                result.ValueGetter = Expression.Lambda<Func<object, object>>(valueProperty, parameter).Compile();

                return result;
            }
            catch
            {
                return default;
            }
        }

        private static bool IsSimpleKeyType(Type type) => type.IsPrimitive ||
                                                          type.IsEnum ||
                                                          type == typeof(string) ||
                                                          type == typeof(Guid);

        private struct DictionaryInfo
        {
            public Type PairType;
            public Func<object, object> KeyGetter;
            public Func<object, object> ValueGetter;
            public bool IsDictionary => PairType != null;
        }
    }
}