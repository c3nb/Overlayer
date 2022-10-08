using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Vostok.Metrics.Models;

namespace Vostok.Metrics.Primitives.Timer
{
    [PublicAPI]
    public class HistogramConfig
    {
        internal static readonly HistogramConfig Default = new HistogramConfig();

        [NotNull]
        public HistogramBuckets Buckets { get; set; }
            = new HistogramBuckets(.005, .01, .025, .05, .075, .1, .25, .5, .75, 1, 2.5, 5, 7.5, 10, 60);

        /// <summary>
        /// See <see cref="MetricEvent.Unit"/> and <see cref="WellKnownUnits"/> for more info.
        /// </summary>
        [CanBeNull]
        [ValueProvider("Vostok.Metrics.WellKnownUnits")]
        public string Unit { get; set; } = WellKnownUnits.Seconds;

        /// <summary>
        /// See <see cref="MetricEvent.AggregationParameters"/> for more info.
        /// </summary>
        [CanBeNull]
        public IReadOnlyDictionary<string, string> AggregationParameters { get; set; }

        /// <summary>
        /// Period of scraping histogram's current value. If left <c>null</c>, context default period will be used.
        /// </summary>
        [CanBeNull]
        public TimeSpan? ScrapePeriod { get; set; }

        /// <summary>
        /// Whether or not to scrape histogram on dispose.
        /// </summary>
        public bool ScrapeOnDispose { get; set; }
    }
}