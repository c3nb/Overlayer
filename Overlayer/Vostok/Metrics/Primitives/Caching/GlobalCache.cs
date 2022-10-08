using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Vostok.Metrics.Models;

namespace Vostok.Metrics.Primitives.Caching
{
    internal static class GlobalCache
    {
        private static readonly ConcurrentDictionary<CacheKey, PerContextCache> PerContextCaches =
            new ConcurrentDictionary<CacheKey, PerContextCache>(new CacheKeyComparer());

        public static TMetric Obtain<TMetric>([NotNull] IMetricContext context, [NotNull] string name, [CanBeNull] object details, [NotNull] Func<TMetric> factory)
        {
            var cacheKey = new CacheKey(context);

            if (cacheKey.BaseContext is DevNullMetricContext)
                return factory();
            
            return (TMetric)PerContextCaches.GetOrAdd(cacheKey, _ => new PerContextCache()).Obtain(name, typeof(TMetric), details, () => factory());
        }

        // note (kungurtsev, 12.11.2021): if we call Obtain after Clean, it will allocate PerContextCaches item again
        // note (kungurtsev, 12.11.2021): but otherwise, it won't be lock-free
        public static void Clean([NotNull] IMetricContext context)
        {
            foreach (var entry in PerContextCaches)
            {
                if (ReferenceEquals(entry.Key.BaseContext, context))
                    PerContextCaches.TryRemove(entry.Key, out _);
            }
        }

        #region CacheKeyComparer

        private class CacheKeyComparer : IEqualityComparer<CacheKey>
        {
            public bool Equals(CacheKey x, CacheKey y) =>
                x.Tags.Equals(y.Tags) && ReferenceEquals(x.BaseContext, y.BaseContext);

            public int GetHashCode(CacheKey key) =>
                (key.Tags.GetHashCode() * 397) ^ RuntimeHelpers.GetHashCode(key.BaseContext);
        }

        #endregion

        #region CacheKey

        private struct CacheKey
        {
            public readonly MetricTags Tags;
            public readonly IMetricContext BaseContext;

            public CacheKey(IMetricContext context)
            {
                Tags = context.Tags;
                BaseContext = context.Unwrap();
            }
        }

        #endregion
    }
}