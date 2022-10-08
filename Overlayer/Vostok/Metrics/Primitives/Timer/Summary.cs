using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using JetBrains.Annotations;
using Vostok.Commons.Threading;
using Vostok.Metrics.Models;
using Vostok.Metrics.Scraping;

namespace Vostok.Metrics.Primitives.Timer
{
    /// <inheritdoc cref="ITimer"/>
    /// <summary>
    /// <para>
    /// Summary allows you to estimate the distribution of values locally, without any server-side aggregation.
    /// </para>
    /// <para>
    /// You configure Summary's by specifying desired quantiles and allowed memory usage.
    /// An online algorithm then computes quantiles on the stream of reported values.
    /// Internal state is periodically scraped to obtain metrics.
    /// </para>
    /// <para>
    /// Summaries are aggregated client-side.
    /// Output of aggregation includes total count, avg and approximate percentiles.
    /// </para>
    /// </summary>
    /// <remarks>
    /// <para>Summary only computes approximate quantiles and is limited to a single process.</para>
    /// <para>Consider using <see cref="Timer"/> or <see cref="Histogram"/> to aggregate values from multiple processes.</para>
    /// <para>Consider using <see cref="Timer"/> to obtain exact values of quantiles.</para>
    /// <para>
    /// To create a Summary, use <see cref="SummaryFactoryExtensions">extensions</see> for <see cref="IMetricContext"/>.
    /// </para>
    /// <para>
    /// Call <see cref="IDisposable.Dispose"/> to stop scraping the metric.
    /// </para>
    /// </remarks>
    internal class Summary : ITimer, IFastScrapableMetric, IDisposable
    {
        private readonly SummaryConfig config;
        private readonly IDisposable registration;

        private readonly QuantileMetricsBuilder quantileBuilder;
        private readonly double[] sample;
        private readonly object snapshotSync = new object();
        private double[] snapshot;
        private int count;

        public Summary([NotNull] IMetricContext context, [NotNull] MetricTags tags, [NotNull] SummaryConfig config)
        {
            this.config = config ?? throw new ArgumentNullException(nameof(config));

            sample = new double[config.BufferSize];
            quantileBuilder = new QuantileMetricsBuilder(config.Quantiles, tags, config.Unit);

            registration = context.Register(this, config.ToScrapableMetricConfig());
        }

        public string Unit => config.Unit;

        public void Report(double value)
        {
            var newCount = Interlocked.Increment(ref count);

            if (newCount <= sample.Length)
            {
                Interlocked.Exchange(ref sample[newCount - 1], value);
            }
            else
            {
                var randomIndex = ThreadSafeRandom.Next(newCount);
                if (randomIndex < sample.Length)
                    Interlocked.Exchange(ref sample[randomIndex], value);
            }
        }

        public IEnumerable<MetricEvent> Scrape(DateTimeOffset timestamp)
        {
            lock (snapshotSync)
            {
                var countBeforeReset = Interlocked.Exchange(ref count, 0);

                var snapshotSize = Math.Min(countBeforeReset, sample.Length);
                if (snapshotSize == 0)
                    return Enumerable.Empty<MetricEvent>();

                if (snapshot == null || snapshot.Length < snapshotSize)
                    snapshot = new double[snapshotSize];

                for (var i = 0; i < snapshotSize; i++)
                    snapshot[i] = Interlocked.CompareExchange(ref sample[i], 0d, 0d);

                return quantileBuilder.Build(snapshot, snapshotSize, countBeforeReset, timestamp);
            }
        }

        public void Dispose()
            => registration.Dispose();
    }
}