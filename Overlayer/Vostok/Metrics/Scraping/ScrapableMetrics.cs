using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Vostok.Commons.Collections;

namespace Vostok.Metrics.Scraping
{
    internal class ScrapableMetrics : IEnumerable<IScrapableMetric>
    {
        private readonly ConcurrentDictionary<IScrapableMetric, byte> metrics
            = new ConcurrentDictionary<IScrapableMetric, byte>(ByReferenceEqualityComparer<IScrapableMetric>.Instance);

        public void Add(IScrapableMetric metric)
            => metrics.TryAdd(metric, byte.MinValue);

        public void Remove(IScrapableMetric metric)
            => metrics.TryRemove(metric, out _);

        public IEnumerator<IScrapableMetric> GetEnumerator()
            => metrics.Select(pair => pair.Key).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();
    }
}