using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Vostok.Metrics.Primitives.Gauge;

namespace Vostok.Metrics.Models
{
    /// <summary>
    /// <para><see cref="MetricDataPoint"/> is a simplified version of <see cref="MetricEvent"/> suitable for direct consumption.</para>
    /// <para>It represents a timestamped value supplied with one or more key-value tags.</para>
    /// <para><see cref="MetricDataPoint"/>s are inevitably converted to <see cref="MetricEvent"/>s at some point with <see cref="ToMetricEvent"/> and exist solely for convenience purposes.</para>
    /// <para>A data point differs from a <see cref="MetricEvent"/> in following key ways:</para>
    /// <list type="bullet">
    ///     <item><description>It always represents a single final point in a time series and does not allow any kind of further aggregation (no <see cref="MetricEvent.AggregationType"/> and <see cref="MetricEvent.AggregationParameters"/>).</description></item>
    ///     <item><description>Its <see cref="Timestamp"/> may be omitted to automatically use current wall clock time.</description></item>
    ///     <item><description>Its <see cref="Tags"/> are meant to be concatenated with contextual static tags (see <see cref="IMetricContext.Tags"/>).</description></item>
    /// </list>
    /// <para>Common ways to use <see cref="MetricDataPoint"/> include <see cref="IMetricContextExtensions_Sending.Send"/> extension and <see cref="MultiFuncGaugeFactoryExtensions"/>.</para>
    /// </summary>
    [PublicAPI]
    public class MetricDataPoint
    {
        /// <param name="value">See <see cref="Value"/>.</param>
        /// <param name="tags">See <see cref="Tags"/>.</param>
        public MetricDataPoint(double value, [NotNull] params (string key, string value)[] tags)
            : this(value, null, tags)
        {
        }

        /// <param name="value">See <see cref="Value"/>.</param>
        /// <param name="name">See <see cref="Name"/>.</param>
        /// <param name="tags">See <see cref="Tags"/>.</param>
        public MetricDataPoint(double value, [CanBeNull] string name, [NotNull] params (string key, string value)[] tags)
        {
            Value = value;
            Name = name;
            Tags = tags;

            if (Tags == null)
                throw new ArgumentNullException(nameof(tags));
        }

        /// <inheritdoc cref="MetricEvent.Value"/>
        public double Value { get; }

        /// <inheritdoc cref="WellKnownTagKeys.Name"/>
        [CanBeNull]
        public string Name { get; }

        /// <inheritdoc cref="MetricEvent.Tags"/>
        [NotNull]
        public IReadOnlyList<(string key, string value)> Tags { get; }

        /// <inheritdoc cref="MetricEvent.Timestamp"/>
        /// <remarks>If set to <c>null</c>, <see cref="ToMetricEvent"/> will use <see cref="DateTimeOffset.Now"/>.</remarks>
        [CanBeNull]
        public DateTimeOffset? Timestamp { get; set; }

        /// <inheritdoc cref="MetricEvent.Unit"/>
        [CanBeNull]
        public string Unit { get; set; }

        [NotNull]
        public MetricEvent ToMetricEvent([NotNull] MetricTags contextTags)
        {
            var metricTags = MetricTagsMerger.Merge(contextTags, Name, ConvertTags());

            if (!metricTags.Any())
                throw new ArgumentException("Tags can't be empty when there's no name and no point tags.", nameof(contextTags));

            return new MetricEvent(
                Value,
                metricTags,
                Timestamp ?? DateTimeOffset.Now,
                Unit,
                null,
                null);
        }

        [NotNull]
        private MetricTag[] ConvertTags()
            => Tags.Select(pair => new MetricTag(pair.key, pair.value)).ToArray();
    }
}