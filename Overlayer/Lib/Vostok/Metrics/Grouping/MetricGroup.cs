using System;
using JetBrains.Annotations;
using Vostok.Metrics.Models;

namespace Vostok.Metrics.Grouping
{
    internal class MetricGroup<TMetric> : MetricGroupBase<TMetric>,
        IMetricGroup1<TMetric>,
        IMetricGroup2<TMetric>,
        IMetricGroup3<TMetric>,
        IMetricGroup4<TMetric>,
        IMetricGroup5<TMetric>,
        IMetricGroup6<TMetric>,
        IMetricGroup7<TMetric>,
        IMetricGroup8<TMetric>
    {
        private readonly string[] keys;

        public MetricGroup([NotNull] Func<MetricTags, TMetric> factory, [NotNull] params string[] keys)
            : base(factory)
        {
            this.keys = keys ?? throw new ArgumentNullException(nameof(keys));
        }

        public TMetric For(string value1)
        {
            var tags = new[]
            {
                new MetricTag(keys[0], value1)
            };

            return For(new MetricTags(tags));
        }

        public TMetric For(string value1, string value2)
        {
            var tags = new[]
            {
                new MetricTag(keys[0], value1),
                new MetricTag(keys[1], value2)
            };

            return For(new MetricTags(tags));
        }

        public TMetric For(string value1, string value2, string value3)
        {
            var tags = new[]
            {
                new MetricTag(keys[0], value1),
                new MetricTag(keys[1], value2),
                new MetricTag(keys[2], value3)
            };

            return For(new MetricTags(tags));
        }

        public TMetric For(string value1, string value2, string value3, string value4)
        {
            var tags = new[]
            {
                new MetricTag(keys[0], value1),
                new MetricTag(keys[1], value2),
                new MetricTag(keys[2], value3),
                new MetricTag(keys[3], value4)
            };

            return For(new MetricTags(tags));
        }

        public TMetric For(string value1, string value2, string value3, string value4, string value5)
        {
            var tags = new[]
            {
                new MetricTag(keys[0], value1),
                new MetricTag(keys[1], value2),
                new MetricTag(keys[2], value3),
                new MetricTag(keys[3], value4),
                new MetricTag(keys[4], value5)
            };

            return For(new MetricTags(tags));
        }

        public TMetric For(string value1, string value2, string value3, string value4, string value5, string value6)
        {
            var tags = new[]
            {
                new MetricTag(keys[0], value1),
                new MetricTag(keys[1], value2),
                new MetricTag(keys[2], value3),
                new MetricTag(keys[3], value4),
                new MetricTag(keys[4], value5),
                new MetricTag(keys[5], value6)
            };

            return For(new MetricTags(tags));
        }

        public TMetric For(string value1, string value2, string value3, string value4, string value5, string value6, string value7)
        {
            var tags = new[]
            {
                new MetricTag(keys[0], value1),
                new MetricTag(keys[1], value2),
                new MetricTag(keys[2], value3),
                new MetricTag(keys[3], value4),
                new MetricTag(keys[4], value5),
                new MetricTag(keys[5], value6),
                new MetricTag(keys[6], value7)
            };

            return For(new MetricTags(tags));
        }

        public TMetric For(string value1, string value2, string value3, string value4, string value5, string value6, string value7, string value8)
        {
            var tags = new[]
            {
                new MetricTag(keys[0], value1),
                new MetricTag(keys[1], value2),
                new MetricTag(keys[2], value3),
                new MetricTag(keys[3], value4),
                new MetricTag(keys[4], value5),
                new MetricTag(keys[5], value6),
                new MetricTag(keys[6], value7),
                new MetricTag(keys[7], value8)
            };

            return For(new MetricTags(tags));
        }
    }
}