using System;
using JetBrains.Annotations;
using Vostok.Metrics.Models;

namespace Vostok.Metrics.Primitives.Timer
{
    /// <inheritdoc cref="ITimer"/>
    /// <summary>
    /// <para>
    /// Timer immediately sends reported values to aggregation backend.
    /// Backend produces several metrics including percentiles, sum and count.
    /// </para>
    /// </summary>
    /// <remarks>
    /// <para>
    /// To create a Timer use <see cref="TimerFactoryExtensions">extensions</see> for <see cref="IMetricContext"/>
    /// </para>
    /// <para>
    /// Timer could be expensive for high workloads (>10k reports per second).
    /// In this case consider using <see cref="Histogram"/>.
    /// </para>
    /// </remarks>
    internal class Timer : ITimer
    {
        private readonly IMetricContext context;
        private readonly MetricTags tags;
        private readonly TimerConfig config;

        public Timer([NotNull] IMetricContext context, [NotNull] MetricTags tags, [NotNull] TimerConfig config)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
            this.tags = tags ?? throw new ArgumentNullException(nameof(tags));
            this.config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public string Unit => config.Unit;

        public void Report(double value)
            => context.Send(new MetricEvent(value, tags, DateTimeOffset.Now, config.Unit, WellKnownAggregationTypes.Timer, config.AggregationParameters));
    }
}