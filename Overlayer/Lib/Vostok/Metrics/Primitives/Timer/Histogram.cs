using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using JetBrains.Annotations;
using Vostok.Metrics.Models;
using Vostok.Metrics.Scraping;

namespace Vostok.Metrics.Primitives.Timer
{
    /// <inheritdoc cref="ITimer"/>
    /// <summary>
    /// <para>
    /// Histogram allows you to estimate the distribution of values.
    /// </para>
    /// <para>
    /// You configure Histogram by specifying buckets.
    /// Each reported value finds the corresponding bucket and increments the value inside.
    /// Then the buckets are periodically scraped.
    /// </para>
    /// <para>
    /// Histograms are aggregated server-side.
    /// Output of aggregation includes total count and percentiles.
    /// </para>
    /// </summary>
    /// <remarks>
    /// <para>
    /// Consider using <see cref="Timer"/> instead of Histogram.
    /// Timer provides exact percentiles and doesn't require any configuration.
    /// However, it is not suitable for high workloads.
    /// </para>
    /// <para>
    /// To create a Histogram use <see cref="HistogramFactoryExtensions">extensions</see> for <see cref="IMetricContext"/>.
    /// </para>
    /// <para>
    /// Build custom <see cref="HistogramBuckets"/> to customize histogram resolution. Avoid creating lots of buckets (> 100).
    /// </para>
    /// <para>
    /// Histogram resets its internal buckets state on each <see cref="Scrape"/> call.
    /// </para>
    /// <para>
    /// Call <see cref="IDisposable.Dispose"/> to stop scraping the metric.
    /// </para>
    /// </remarks>
    internal class Histogram : ITimer, IFastScrapableMetric, IDisposable
    {
        private readonly HistogramConfig config;
        private readonly IDisposable registration;
        private readonly MetricTags tags;
        private readonly long[] bucketCounters;
        private readonly IReadOnlyDictionary<string, string>[] aggregationParameters;
        private readonly HistogramBuckets buckets;

        public Histogram([NotNull] IMetricContext context, [NotNull] MetricTags tags, [NotNull] HistogramConfig config)
        {
            this.config = config ?? throw new ArgumentNullException(nameof(config));
            this.tags = tags ?? throw new ArgumentNullException(nameof(tags));

            buckets = config.Buckets;
            bucketCounters = new long[buckets.Count];
            aggregationParameters = new IReadOnlyDictionary<string, string>[buckets.Count];

            for (var i = 0; i < buckets.Count; i++)
            {
                var parameters = config.AggregationParameters?.ToDictionary(x => x.Key, x => x.Value) ?? new Dictionary<string, string>();
                parameters.SetHistogramBucket(buckets[i]);
                aggregationParameters[i] = parameters;
            }

            registration = context.Register(this, config.ToScrapableMetricConfig());
        }

        public string Unit => config.Unit;

        public void Report(double value)
        {
            if (double.IsNaN(value))
                return;

            Interlocked.Increment(ref bucketCounters[buckets.FindBucketIndex(value)]);
        }

        public IEnumerable<MetricEvent> Scrape(DateTimeOffset timestamp)
        {
            for (var i = 0; i < bucketCounters.Length; i++)
            {
                var bucketValue = Interlocked.Exchange(ref bucketCounters[i], 0L);
                if (bucketValue != 0)
                    yield return new MetricEvent(bucketValue, tags, timestamp, config.Unit, WellKnownAggregationTypes.Histogram, aggregationParameters[i]);
            }
        }

        public void Dispose()
            => registration.Dispose();
    }
}