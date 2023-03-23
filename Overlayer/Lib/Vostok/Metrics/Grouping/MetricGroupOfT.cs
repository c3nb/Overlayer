using System;
using Vostok.Metrics.Models;

namespace Vostok.Metrics.Grouping
{
    internal class MetricGroup<TFor, TMetric> : MetricGroupBase<TMetric>, IMetricGroup<TFor, TMetric>
    {
        public MetricGroup(Func<MetricTags, TMetric> factory)
            : base(factory)
        {
            if (!MetricTagsExtractor.HasTags(typeof(TFor)))
                throw new ArgumentException($"Tags model type '{typeof(TFor).Name}' doesn't have any public properties marked with '{typeof(MetricTagAttribute).Name}'.");
        }

        public TMetric For(TFor value)
            => For(MetricTagsExtractor.ExtractTags(value));
    }
}