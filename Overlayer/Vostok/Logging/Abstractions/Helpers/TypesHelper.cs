using System;
using Vostok.Commons.Collections;

namespace Vostok.Logging.Abstractions.Helpers
{
    internal static class TypesHelper
    {
        private static readonly RecyclingBoundedCache<Type, bool> Cache = new RecyclingBoundedCache<Type, bool>(10000);

        public static bool IsAnonymousType(Type type)
            => Cache.Obtain(type, IsAnonymousTypeInternal);
        
        private static bool IsAnonymousTypeInternal(Type type)
            => type.IsConstructedGenericType &&
               Nullable.GetUnderlyingType(type) == null &&
               type.Name.Contains("<>") &&
               type.Name.Contains("AnonymousType");
    }
}