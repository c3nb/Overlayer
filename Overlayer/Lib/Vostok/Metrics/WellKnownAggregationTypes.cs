using JetBrains.Annotations;
using Vostok.Metrics.Models;

namespace Vostok.Metrics
{
    /// <summary>
    /// Names for some of well-recognized <see cref="MetricEvent.AggregationType">aggregation types</see>.
    /// </summary>
    [PublicAPI]
    public static class WellKnownAggregationTypes
    {
        /// <summary>
        /// <para>No preliminary aggregation is performed on client side: every reported metric event is sent to storage.</para>
        /// <para>Server-side aggregation infrastructure computes following aggregates:</para>
        /// <list type="bullet">
        ///     <item><description>Rate (count per unit of time)</description></item>
        ///     <item><description>Average value</description></item>
        ///     <item><description>A set of exact quantiles (p50, p75, p95, p99)</description></item>
        /// </list>
        /// <para>Aggregates are computed over configurable time windows.</para>
        /// </summary>
        public const string Timer = "timer";

        /// <summary>
        /// <para>Preliminary aggregation is performed on client side: every reported value increments a counter in one of the preconfigured buckets.</para>
        /// <para>Server side infrastructure then merges histograms of different processes and computes following aggregates:</para>
        /// <list type="bullet">
        ///     <item><description>Rate (count per unit of time)</description></item>
        ///     <item><description>A set of approximate quantiles (p50, p75, p95, p99) with values interpolated from bucket boundaries.</description></item>
        /// </list>
        /// <para>Aggregates are computed over configurable time windows.</para>
        /// </summary>
        public const string Histogram = "histogram";

        /// <summary>
        /// <para>No preliminary aggregation is performed on client side: every reported metric event is sent to storage.</para>
        /// <para>Server-side infrastructure computes a count aggregate by summing all the values.</para>
        /// <para>Aggregates are computed over configurable time windows.</para>
        /// </summary>
        public const string Counter = "counter";
    }
}