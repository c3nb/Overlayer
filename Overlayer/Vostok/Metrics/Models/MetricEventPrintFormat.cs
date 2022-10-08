using JetBrains.Annotations;

namespace Vostok.Metrics.Models
{
    [PublicAPI]
    public enum MetricEventPrintFormat
    {
        /// <summary>
        /// Full JSON-based representation of <see cref="MetricEvent"/>.
        /// </summary>
        Json,

        /// <summary>
        /// One-line representation that omits tag keys, timestamps and aggregation data, strongly resembling Graphite-like flat metric names.
        /// </summary>
        Flat
    }
}
