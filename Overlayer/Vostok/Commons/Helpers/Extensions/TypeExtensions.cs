using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;

namespace Vostok.Commons.Helpers.Extensions
{
    [PublicAPI]
    internal static class TypeExtensions
    {
        public static bool IsNullable(this Type type) => Nullable.GetUnderlyingType(type) != null;

        public static IEnumerable<PropertyInfo> GetInstanceProperties(this Type type) =>
            type.GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(p => p.GetIndexParameters().Length == 0);

        public static IEnumerable<FieldInfo> GetInstanceFields(this Type type) =>
            type.GetFields(BindingFlags.Instance | BindingFlags.Public);
    }
}