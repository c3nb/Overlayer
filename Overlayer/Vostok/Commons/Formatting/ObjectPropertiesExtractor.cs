using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using JetBrains.Annotations;
using Vostok.Commons.Collections;

// ReSharper disable ConvertClosureToMethodGroup

namespace Vostok.Commons.Formatting
{
    [PublicAPI]
    internal static class ObjectPropertiesExtractor
    {
        private const int CacheCapacity = 10000;

        private static readonly RecyclingBoundedCache<Type, (string name, Func<object, object> getter)[]> Cache =
            new RecyclingBoundedCache<Type, (string name, Func<object, object> getter)[]>(CacheCapacity);

        public static bool HasProperties(Type type) =>
            GetPropertiesCount(type) > 0;

        public static int GetPropertiesCount(Type type) =>
            Cache.Obtain(type, obj => LocateProperties(obj)).Length;

        public static IEnumerable<(string, object)> ExtractProperties(object @object)
        {
            foreach (var (name, getter) in Cache.Obtain(@object.GetType(), obj => LocateProperties(obj)))
                yield return (name, ObtainPropertyValue(@object, getter));
        }

        public static (int, IEnumerable<(string, object)>) ExtractPropertiesWithCount(object @object)
        {
            var pairs = Cache.Obtain(@object.GetType(), obj => LocateProperties(obj));
            return (pairs.Length, Get(@object, pairs));
        }

        private static IEnumerable<(string, object)> Get(object @object, (string name, Func<object, object> getter)[] pairs)
        {
            foreach (var (name, getter) in pairs)
                yield return (name, ObtainPropertyValue(@object, getter));
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

        private static object ObtainPropertyValue(object @object, Func<object, object> getter)
        {
            try
            {
                return getter(@object);
            }
            catch (Exception error)
            {
                return $"<error: {error.Message}>";
            }
        }
    }
}