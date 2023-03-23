using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Vostok.Metrics.Models;

namespace Vostok.Metrics.Primitives.Counter
{
    [PublicAPI]
    public class CounterConfig
    {
        internal static readonly CounterConfig Default = new CounterConfig();

        /// <summary>
        /// See <see cref="MetricEvent.Unit"/> and <see cref="WellKnownUnits"/> for more info.
        /// </summary>
        [CanBeNull]
        [ValueProvider("Vostok.Metrics.WellKnownUnits")]
        public string Unit { get; set; }

        /// <summary>
        /// See <see cref="MetricEvent.AggregationParameters"/> for more info.
        /// </summary>
        [CanBeNull]
        public IReadOnlyDictionary<string, string> AggregationParameters { get; set; }

        /// <summary>
        /// Whether or not to send counter with zero value.
        /// </summary>
        public bool SendZeroValues { get; set; } = true;

        /// <summary>
        /// Period of scraping counter's current value. If set to <c>null</c>, context default period will be used.
        /// </summary>
        [CanBeNull]
        public TimeSpan? ScrapePeriod { get; set; }

        /// <summary>
        /// Whether or not to scrape counter on dispose.
        /// </summary>
        public bool ScrapeOnDispose { get; set; } = true;
    }
}