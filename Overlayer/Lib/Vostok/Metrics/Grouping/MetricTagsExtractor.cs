using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Vostok.Commons.Collections;
using Vostok.Metrics.Models;

namespace Vostok.Metrics.Grouping
{
    internal static class MetricTagsExtractor
    {
        private const int CacheCapacity = 100;

        private static readonly RecyclingBoundedCache<Type, (string name, Func<object, object> getter)[]> Cache =
            new RecyclingBoundedCache<Type, (string name, Func<object, object> getter)[]>(CacheCapacity);

        public static bool HasTags(Type type) =>
            Cache.Obtain(type, LocateProperties).Length > 0;

        public static MetricTags ExtractTags(object item)
        {
            var properties = Cache.Obtain(item.GetType(), LocateProperties);
            var tags = new MetricTag[properties.Length];

            for (var i = 0; i < properties.Length; i++)
                tags[i] = new MetricTag(properties[i].name, ObtainPropertyValue(item, properties[i].getter));

            return new MetricTags(tags);
        }

        private static (string, Func<object, object>)[] LocateProperties(Type type)
        {
            try
            {
                var properties = type
                    .GetTypeInfo()
                    .DeclaredProperties
                    .Where(property => property.CanRead)
                    .Where(property => property.GetMethod.IsPublic)
                    .Where(property => !property.GetMethod.IsStatic)
                    .Where(property => !property.GetIndexParameters().Any())
                    .Where(property => property.GetCustomAttribute<MetricTagAttribute>() != null)
                    .OrderBy(property => property.GetCustomAttribute<MetricTagAttribute>().Order)
                    .ToArray();

                var getters = new (string, Func<object, object>)[properties.Length];

                for (var i = 0; i < properties.Length; i++)
                {
                    var parameter = Expression.Parameter(typeof(object));
                    var convertedParameter = Expression.Convert(parameter, type);

                    var property = Expression.Property(convertedParameter, properties[i]);
                    var convertedProperty = Expression.Convert(property, typeof(object));

                    getters[i] = (properties[i].Name, Expression.Lambda<Func<object, object>>(convertedProperty, parameter).Compile());
                }

                return getters;
            }
            catch
            {
                return Array.Empty<(string, Func<object, object>)>();
            }
        }

        private static string ObtainPropertyValue(object item, Func<object, object> getter)
        {
            try
            {
                var value = getter(item)?.ToString();

                if (string.IsNullOrEmpty(value))
                    return "none";

                return value;
            }
            catch
            {
                return "none";
            }
        }
    }
}