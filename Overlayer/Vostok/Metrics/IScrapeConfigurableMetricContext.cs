using System;
using JetBrains.Annotations;
using Vostok.Metrics.Scraping;

namespace Vostok.Metrics
{
    internal interface IScrapeConfigurableMetricContext
    {
        /// <inheritdoc cref="IMetricContext.Register"/>
        /// <param name="scrapableMetricConfig">Scrape configuration</param>
        [NotNull]
        IDisposable Register([NotNull] IScrapableMetric metric, [CanBeNull] ScrapableMetricConfig scrapableMetricConfig);
    }
}