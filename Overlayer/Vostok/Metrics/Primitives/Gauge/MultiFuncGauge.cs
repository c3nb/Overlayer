using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Vostok.Metrics.Models;
using Vostok.Metrics.Scraping;

namespace Vostok.Metrics.Primitives.Gauge
{
    internal class MultiFuncGauge : IScrapableMetric, IMultiFuncGauge, IDisposable
    {
        private readonly MetricTags contextTags;
        private readonly FuncGaugeConfig config;
        private readonly IDisposable registration;
        private readonly Func<IEnumerable<MetricDataPoint>> pointsProvider;
        private readonly Func<IEnumerable<MetricEvent>> eventsProvider;

        public MultiFuncGauge(
            [NotNull] IMetricContext context,
            [NotNull] Func<IEnumerable<MetricEvent>> eventsProvider,
            [NotNull] FuncGaugeConfig config)
            : this(context, config) =>
            this.eventsProvider = eventsProvider ?? throw new ArgumentNullException(nameof(eventsProvider));

        public MultiFuncGauge(
            [NotNull] IMetricContext context,
            [NotNull] Func<IEnumerable<MetricDataPoint>> pointsProvider,
            [NotNull] FuncGaugeConfig config)
            : this(context, config) =>
            this.pointsProvider = pointsProvider ?? throw new ArgumentNullException(nameof(pointsProvider));

        private MultiFuncGauge(
            [NotNull] IMetricContext context,
            [NotNull] FuncGaugeConfig config)
        {
            contextTags = context.Tags;
            this.config = config ?? throw new ArgumentNullException(nameof(config));

            registration = context.Register(this, config.ToScrapableMetricConfig());
        }

        public void Dispose()
            => registration.Dispose();

        public IEnumerable<MetricEvent> Scrape(DateTimeOffset timestamp)
        {
            if (pointsProvider != null)
                return pointsProvider().Select(point => CreateEvent(point, timestamp));
            if (eventsProvider != null)
                return eventsProvider().Select(@event => CreateEvent(@event, timestamp));
            return Array.Empty<MetricEvent>();
        }

        private MetricEvent CreateEvent(MetricDataPoint point, DateTimeOffset timestamp)
        {
            point.Timestamp = timestamp;
            point.Unit = point.Unit ?? config.Unit;

            return point.ToMetricEvent(contextTags);
        }

        private MetricEvent CreateEvent(MetricEvent @event, DateTimeOffset timestamp)
        {
            return new MetricEvent(
                @event.Value,
                contextTags.Append(@event.Tags),
                timestamp,
                @event.Unit,
                @event.AggregationType,
                @event.AggregationParameters);
        }
    }
}