using System;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using Vostok.Commons.Collections;

// ReSharper disable ReplaceWithSingleCallToFirstOrDefault

namespace Vostok.Commons.Formatting
{
    [PublicAPI]
    internal static class ParseMethodFinder
    {
        private const int CacheCapacity = 1000;
        
        private static readonly RecyclingBoundedCache<Type, MethodInfo> parseMethods 
            = new RecyclingBoundedCache<Type, MethodInfo>(CacheCapacity);

        private static readonly RecyclingBoundedCache<Type, MethodInfo> tryParseMethods
            = new RecyclingBoundedCache<Type, MethodInfo>(CacheCapacity);

        public static bool HasAnyKindOfParseMethod([NotNull] Type type)
            => (FindTryParseMethod(type) ?? FindParseMethod(type)) != null;

        [CanBeNull]
        public static MethodInfo FindParseMethod([NotNull] Type type)
            => parseMethods.Obtain(type, FindParseMethodInternal);

        [CanBeNull]
        public static MethodInfo FindTryParseMethod([NotNull] Type type)
            => tryParseMethods.Obtain(type, FindTryParseMethodInternal);

        [CanBeNull]
        private static MethodInfo FindParseMethodInternal([NotNull] Type type)
            => type
                .GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Where(method => method.Name == nameof(int.Parse))
                .Where(method => method.ReturnType == type)
                .Where(method => !method.IsGenericMethod)
                .Where(method => method.GetParameters().Length == 1)
                .Where(method => method.GetParameters()[0].ParameterType == typeof(string))
                .FirstOrDefault();

        [CanBeNull]
        private static MethodInfo FindTryParseMethodInternal([NotNull] Type type)
            => type
                .GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Where(method => method.Name == nameof(int.TryParse))
                .Where(method => method.ReturnType == typeof(bool))
                .Where(method => !method.IsGenericMethod)
                .Where(method => method.GetParameters().Length == 2)
                .Where(method => method.GetParameters()[0].ParameterType == typeof(string))
                .Where(method => method.GetParameters()[1].IsOut)
                .Where(method => method.GetParameters()[1].ParameterType == type.MakeByRefType())
                .FirstOrDefault();
    }
}