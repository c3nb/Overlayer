using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Vostok.Metrics.Models;
using Vostok.Metrics.Scraping;

// ReSharper disable ParameterHidesMember

namespace Vostok.Metrics.Primitives.Gauge
{
    internal class FuncGauge : IScrapableMetric, IFuncGauge, IDisposable
    {
        private readonly MetricTags tags;
        private readonly FuncGaugeConfig config;
        private readonly IDisposable registration;
        private volatile Func<double?> valueProvider;

        public FuncGauge(
            [NotNull] IMetricContext context,
            [NotNull] MetricTags tags,
            [CanBeNull] Func<double?> valueProvider,
            [NotNull] FuncGaugeConfig config)
        {
            this.valueProvider = valueProvider;
            this.tags = tags ?? throw new ArgumentNullException(nameof(tags));
            this.config = config ?? throw new ArgumentNullException(nameof(config));

            registration = context.Register(this, config.ToScrapableMetricConfig());
        }

        public FuncGauge(
            [NotNull] IMetricContext context,
            [NotNull] MetricTags tags,
            [NotNull] FuncGaugeConfig config)
            : this(context, tags, null, config)
        {
        }

        public IEnumerable<MetricEvent> Scrape(DateTimeOffset timestamp)
        {
            var value = valueProvider?.Invoke();
            if (value != null && (value != 0 || config.SendZeroValues))
                yield return new MetricEvent(value.Value, tags, timestamp, config.Unit, null, null);
        }

        public void SetValueProvider(Func<double> valueProvider)
        {
            if (valueProvider == null)
                throw new ArgumentNullException(nameof(valueProvider));

            this.valueProvider = () => valueProvider();
        }

        public void SetValueProvider(Func<double?> valueProvider)
        {
            this.valueProvider = valueProvider ?? throw new ArgumentNullException(nameof(valueProvider));
        }

        public void Dispose()
            => registration.Dispose();
    }
}