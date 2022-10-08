using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Vostok.Commons.Helpers.Comparers;

namespace Vostok.Metrics.Models
{
    /// <summary>
    /// <para><see cref="MetricEvent"/> is the atomic storage unit of Vostok metrics system.</para>
    /// <para>Every event contains a numeric <see cref="Value"/> measured at some <see cref="Timestamp"/> and bound to a set of <see cref="Tags"/>.</para>
    /// <para>Events may also contain auxiliary information, such as <see cref="Unit"/>, <see cref="AggregationType"/> and <see cref="AggregationParameters"/>.</para>
    /// <para><see cref="MetricEvent"/> instances are immutable.</para>
    /// <para><see cref="MetricEvent"/>s are not meant to be instantiated manually. It's recommended to either use one of the primitives or utilize <see cref="MetricDataPoint"/>.</para>
    /// </summary>
    [PublicAPI]
    public class MetricEvent : IEquatable<MetricEvent>
    {
        /// <param name="value">See <see cref="Value"/>.</param>
        /// <param name="tags">See <see cref="Tags"/>.</param>
        /// <param name="timestamp">See <see cref="Timestamp"/>.</param>
        /// <param name="unit">See <see cref="Unit"/>.</param>
        /// <param name="aggregationType">See <see cref="AggregationType"/>.</param>
        /// <param name="aggregationParameters">See <see cref="AggregationParameters"/>.</param>
        public MetricEvent(
            double value,
            [NotNull] MetricTags tags,
            DateTimeOffset timestamp,
            [CanBeNull] [ValueProvider("Vostok.Metrics.WellKnownUnits")]
            string unit,
            [CanBeNull] [ValueProvider("Vostok.Metrics.WellKnownAggregationTypes")]
            string aggregationType,
            [CanBeNull] IReadOnlyDictionary<string, string> aggregationParameters)
        {
            Tags = tags ?? throw new ArgumentNullException(nameof(tags));

            if (tags.Count == 0)
                throw new ArgumentException("Empty tags are not allowed in metric events.", nameof(tags));

            Unit = unit;
            Value = value;
            Timestamp = timestamp;
            AggregationType = aggregationType;
            AggregationParameters = aggregationParameters;
        }

        /// <summary>
        /// Numeric value of the metric represented by this <see cref="MetricEvent"/>.
        /// </summary>
        public double Value { get; }

        /// <summary>
        /// Timestamp denoting the moment when this <see cref="MetricEvent"/>'s <see cref="Value"/> was observed.
        /// </summary>
        public DateTimeOffset Timestamp { get; }

        /// <summary>
        /// <para>Set of key-value string tags this event's <see cref="Value"/> is bound to. May not be empty.</para>
        /// <para>See <see cref="MetricTags"/> for more details.</para>
        /// </summary>
        [NotNull]
        public MetricTags Tags { get; }

        /// <summary>
        /// <para>Unit of measurement this event's <see cref="Value"/> is expressed in.</para>
        /// <para>Dimensionless quantities (such as count) may have no unit (<c>null</c> value).</para>
        /// <para>See <see cref="WellKnownUnits"/> for predefined unit constants.</para>
        /// </summary>
        [CanBeNull]
        public string Unit { get; }

        /// <summary>
        /// <para>Aggregation type defines what happens to the event after it's constructed and sent.</para>
        /// <para><c>Null</c> value means no aggregation: event will be published to the metrics storage of choice (like Graphite or Prometheus) as is.</para>
        /// <para>Any non-null value implies server-side processing: event will undergo some kind of aggregation that will eventually produce other events with <c>null</c> <see cref="AggregationType"/>.</para>
        /// <para>See <see cref="WellKnownAggregationTypes"/> for predefined type constants.</para>
        /// </summary>
        [CanBeNull]
        public string AggregationType { get; }

        /// <summary>
        /// <para>An optional set of key-value string pairs that may contain customization parameters for server-side aggregation mechanism.</para>
        /// <para>Contents of <see cref="AggregationParameters"/> are specific to selected <see cref="AggregationType"/>.</para>
        /// </summary>
        [CanBeNull]
        public IReadOnlyDictionary<string, string> AggregationParameters { get; }

        public override string ToString() => EventPrinter.Print(this, MetricEventPrintFormat.Json);

        #region Equality

        public bool Equals(MetricEvent other)
        {
            if (other == null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            if (!Value.Equals(other.Value))
                return false;

            if (!Timestamp.UtcTicks.Equals(other.Timestamp.UtcTicks))
                return false;

            if (!Tags.Equals(other.Tags))
                return false;

            if (!string.Equals(Unit, other.Unit))
                return false;

            if (!string.Equals(AggregationType, other.AggregationType))
                return false;

            return DictionaryComparer<string, string>.Instance.Equals(AggregationParameters, other.AggregationParameters);
        }

        public override bool Equals(object obj)
            => Equals(obj as MetricEvent);

        // ReSharper disable once AssignNullToNotNullAttribute
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Value.GetHashCode();
                hashCode = (hashCode * 397) ^ Timestamp.UtcTicks.GetHashCode();
                hashCode = (hashCode * 397) ^ Tags.GetHashCode();
                hashCode = (hashCode * 397) ^ (Unit?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (AggregationType?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ DictionaryComparer<string, string>.Instance.GetHashCode(AggregationParameters);
                return hashCode;
            }
        }

        #endregion
    }
}