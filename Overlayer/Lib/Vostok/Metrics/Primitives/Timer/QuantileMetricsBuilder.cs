using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Vostok.Metrics.Models;

namespace Vostok.Metrics.Primitives.Timer
{
    /// <summary>
    /// Builds array of <see cref="MetricEvent"/>'s from array of values and quantiles.
    /// </summary>
    [PublicAPI]
    public class QuantileMetricsBuilder
    {
        private readonly MetricTags tags;
        private readonly MetricTags countTags;
        private readonly MetricTags averageTags;

        private readonly string unit;
        private readonly double[] quantiles;
        private readonly MetricTags[] quantileTags;

        /// <summary>
        /// If <paramref name="quantiles"/> is <c>null</c>, <see cref="Quantiles.DefaultQuantiles"/> will be used.
        /// </summary>
        public QuantileMetricsBuilder([CanBeNull] double[] quantiles, [NotNull] MetricTags tags, [CanBeNull] string unit)
        {
            this.tags = tags;
            this.unit = unit;
            this.quantiles = quantiles = quantiles ?? Quantiles.DefaultQuantiles;

            quantileTags = Quantiles.QuantileTags(quantiles, tags);

            countTags = tags.Append(WellKnownTagKeys.Aggregate, WellKnownTagValues.AggregateCount);
            averageTags = tags.Append(WellKnownTagKeys.Aggregate, WellKnownTagValues.AggregateAverage);
        }

        public IEnumerable<MetricEvent> Build(List<double> values, DateTimeOffset timestamp)
        {
            values.Sort();
            return BuildForSorted(values, values.Count, values.Count, timestamp);
        }

        public IEnumerable<MetricEvent> Build(double[] values, DateTimeOffset timestamp)
            => Build(values, values.Length, values.Length, timestamp);

        public IEnumerable<MetricEvent> Build(double[] values, int size, int totalCount, DateTimeOffset timestamp)
        {
            Array.Sort(values, 0, size);
            return BuildForSorted(values, size, totalCount, timestamp);
        }

        private static double GetAverage(IList<double> values, int size)
            => size == 0 ? 0 : values.Take(size).Average();

        private IEnumerable<MetricEvent> BuildForSorted(IList<double> values, int size, int totalCount, DateTimeOffset timestamp)
        {
            var result = new List<MetricEvent>
            {
                new MetricEvent(totalCount, countTags, timestamp, null, null, null),
                new MetricEvent(GetAverage(values, size), averageTags, timestamp, unit, null, null)
            };

            for (var i = 0; i < quantiles.Length; i++)
            {
                result.Add(
                    new MetricEvent(
                        Quantiles.GetQuantile(quantiles[i], values, size),
                        quantileTags[i],
                        timestamp,
                        unit,
                        null,
                        null));
            }

            return result;
        }
    }
}