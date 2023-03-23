using System;
using System.Collections.Generic;
using System.Threading;
using JetBrains.Annotations;
using Vostok.Commons.Threading;
using Vostok.Metrics.Models;
using Vostok.Metrics.Scraping;

namespace Vostok.Metrics.Primitives.Gauge
{
    /// <inheritdoc cref="IIntegerGauge"/>
    internal class IntegerGauge : IFastScrapableMetric, IIntegerGauge, IDisposable
    {
        private readonly MetricTags tags;
        private readonly IntegerGaugeConfig config;
        private readonly IDisposable registration;
        private readonly AtomicBoolean valueModified;
        private long value;

        public IntegerGauge(
            [NotNull] IMetricContext context,
            [NotNull] MetricTags tags,
            [NotNull] IntegerGaugeConfig config)
        {
            this.tags = tags ?? throw new ArgumentNullException(nameof(tags));
            this.config = config ?? throw new ArgumentNullException(nameof(config));

            if (!this.config.SendInitialValue && this.config.ResetOnScrape)
                throw new ArgumentException($"'{nameof(IntegerGaugeConfig)}.{nameof(IntegerGaugeConfig.SendInitialValue)} = false' is incompatible with '{nameof(IntegerGaugeConfig)}.{nameof(IntegerGaugeConfig.ResetOnScrape)} = true'.");

            value = config.InitialValue;
            valueModified = false;

            registration = context.Register(this, config.ToScrapableMetricConfig());
        }

        public long CurrentValue => Interlocked.Read(ref value);

        public IEnumerable<MetricEvent> Scrape(DateTimeOffset timestamp)
        {
            var valueToReport = config.ResetOnScrape
                ? Interlocked.Exchange(ref value, config.InitialValue)
                : CurrentValue;

            if ((config.SendZeroValues || valueToReport != 0) && (config.SendInitialValue || valueModified))
                yield return new MetricEvent(valueToReport, tags, timestamp, config.Unit, null, null);
        }

        public void Set(long newValue)
        {
            Interlocked.Exchange(ref value, newValue);
            valueModified.SetTrue();
        }

        public void Increment()
        {
            Interlocked.Increment(ref value);
            valueModified.SetTrue();
        }

        public void Decrement()
        {
            Interlocked.Decrement(ref value);
            valueModified.SetTrue();
        }

        public void Add(long valueToAdd)
        {
            Interlocked.Add(ref value, valueToAdd);
            valueModified.SetTrue();
        }

        public void Substract(long valueToSubstract) =>
            Add(-valueToSubstract);

        public void TryIncreaseTo(long candidateValue)
        {
            while (true)
            {
                var currentValue = CurrentValue;
                if (candidateValue <= currentValue || TrySet(candidateValue, currentValue))
                    return;
            }
        }

        public void TryReduceTo(long candidateValue)
        {
            while (true)
            {
                var currentValue = CurrentValue;
                if (candidateValue >= currentValue || TrySet(candidateValue, currentValue))
                    return;
            }
        }

        public void Dispose()
        {
            registration.Dispose();
        }

        private bool TrySet(long newValue, long expectedValue)
        {
            var result = Interlocked.CompareExchange(ref value, newValue, expectedValue) == expectedValue;
            if (result)
                valueModified.SetTrue();
            return result;
        }
    }
}