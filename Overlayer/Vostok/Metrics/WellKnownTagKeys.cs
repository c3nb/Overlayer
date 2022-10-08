using JetBrains.Annotations;
using Vostok.Metrics.Models;
using Vostok.Metrics.Primitives.Timer;

namespace Vostok.Metrics
{
    /// <summary>
    /// Names for some of well-recognized metric tag <see cref="MetricTag.Key">keys</see>.
    /// </summary>
    [PublicAPI]
    public static class WellKnownTagKeys
    {
        /// <summary>
        /// <para><see cref="Name"/> tag represents the physical meaning of the metric being gathered.</para>
        /// <para>It's value usually allows to infer the nature of corresponding measurement unit.</para>
        /// <para>Here are some valid examples of metric names: <c>'requestsPerSecond'</c>, <c>'queueSize'</c>, <c>'writeLatency'</c>.</para>
        /// </summary>
        public const string Name = "_name";

        /// <summary>
        /// <para><see cref="LowerBound"/> tag represents numerical exclusive lower bound for a histogram bucket.</para>
        /// <para>It has a special value of <c>-Inf</c> (see <see cref="WellKnownTagValues.NegativeInfinity"/>) to denote negative infinity.</para>
        /// <para>See <see cref="HistogramFactoryExtensions"/> to learn more about histograms.</para>
        /// </summary>
        public const string LowerBound = "_lowerBound";

        /// <summary>
        /// <para><see cref="UpperBound"/> tag represents numerical inclusive upper bound for a histogram bucket.</para>
        /// <para>It has a special value of <c>+Inf</c> (see <see cref="WellKnownTagValues.PositiveInfinity"/>) to denote positive infinity.</para>
        /// <para>See <see cref="HistogramFactoryExtensions"/> to learn more about histograms.</para>
        /// </summary>
        public const string UpperBound = "_upperBound";

        /// <summary>
        /// <para><see cref="Aggregate"/> tag represents a type of numerical aggregate applied to raw values to obtain <see cref="MetricEvent"/>'s <see cref="MetricEvent.Value"/>.</para>
        /// <para>It's commonly encountered in events produced by <c>Summary</c> primitive and external aggregators that handle timers, histograms and counters.</para>
        /// <para>Common well-known values for this tag are:</para>
        /// <list type="bullet">
        ///     <item><description><c>count</c></description></item>
        ///     <item><description><c>avg</c></description></item>
        ///     <item><description><c>p50</c></description></item>
        ///     <item><description><c>p75</c></description></item>
        ///     <item><description><c>p95</c></description></item>
        ///     <item><description><c>p99</c></description></item>
        ///     <item><description><c>p999</c></description></item>
        /// </list>
        /// </summary>
        public const string Aggregate = "_aggregate";

        /// <summary>
        /// <see cref="Component"/> tag represents the application's subsystem responsible for producing metrics.
        /// </summary>
        public const string Component = "component";
    }
}