using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Vostok.Metrics.Models;

namespace Vostok.Metrics.Scraping
{
    /// <summary>
    /// Represents a metric whose state can be periodically observed.
    /// </summary>
    [PublicAPI]
    public interface IScrapableMetric
    {
        /// <summary>
        /// <para>Converts internal metric state to one or more <see cref="MetricEvent"/>s.</para>
        /// <para>Implementation could reset the state after this method was called.</para>
        /// </summary>
        /// <param name="timestamp">Timestamp to include for returned <see cref="MetricEvent"/>s, denoting exact scraping moment.</param>
        /// <returns><see cref="MetricEvent"/>s describing current state of the metric.</returns>
        [NotNull]
        [ItemNotNull]
        IEnumerable<MetricEvent> Scrape(DateTimeOffset timestamp);
    }
}