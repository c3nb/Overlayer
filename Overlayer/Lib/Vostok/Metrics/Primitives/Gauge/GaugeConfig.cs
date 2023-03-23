using System;
using JetBrains.Annotations;
using Vostok.Metrics.Models;

namespace Vostok.Metrics.Primitives.Gauge
{
    [PublicAPI]
    public class GaugeConfig
    {
        /// <summary>
        /// See <see cref="MetricEvent.Unit"/> and <see cref="WellKnownUnits"/> for more info.
        /// </summary>
        [CanBeNull]
        [ValueProvider("Vostok.Metrics.WellKnownUnits")]
        public string Unit { get; set; }

        /// <summary>
        /// Period of scraping gauge's current value. If left <c>null</c>, context default period will be used.
        /// </summary>
        [CanBeNull]
        public TimeSpan? ScrapePeriod { get; set; }

        /// <summary>
        /// Whether or not to scrape gauge on dispose.
        /// </summary>
        public bool ScrapeOnDispose { get; set; }

        /// <summary>
        /// Whether or not to send gauge with zero value.
        /// </summary>
        public bool SendZeroValues { get; set; } = true;
    }
}