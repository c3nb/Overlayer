using System;
using System.Collections.Concurrent;
using System.Threading;
using JetBrains.Annotations;
using Vostok.Metrics.Models;

namespace Vostok.Metrics.Grouping
{
    internal abstract class MetricGroupBase<TMetric> : IDisposable
    {
        private readonly ConcurrentDictionary<MetricTags, Lazy<TMetric>> cache = new ConcurrentDictionary<MetricTags, Lazy<TMetric>>();
        private readonly Func<MetricTags, TMetric> factory;

        protected MetricGroupBase([NotNull] Func<MetricTags, TMetric> factory)
            => this.factory = factory ?? throw new ArgumentNullException(nameof(factory));

        public void Dispose()
        {
            foreach (var pair in cache)
            {
                (pair.Value.Value as IDisposable)?.Dispose();

                cache.TryRemove(pair.Key, out _);
            }
        }

        protected TMetric For([NotNull] MetricTags dynamicTags)
            => cache.GetOrAdd(dynamicTags, t => new Lazy<TMetric>(() => factory(t), LazyThreadSafetyMode.ExecutionAndPublication)).Value;
    }
}