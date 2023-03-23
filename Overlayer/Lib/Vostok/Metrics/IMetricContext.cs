using System;
using JetBrains.Annotations;
using Vostok.Metrics.Models;
using Vostok.Metrics.Primitives.Counter;
using Vostok.Metrics.Primitives.Gauge;
using Vostok.Metrics.Primitives.Timer;
using Vostok.Metrics.Scraping;

namespace Vostok.Metrics
{
    /// <summary>
    /// <para><see cref="IMetricContext"/> is an entry-point for Vostok.Metrics library.</para>
    /// <para>It provides all that is necessary to create a metric primitive like <see cref="IIntegerGauge"/>, <see cref="ICounter"/>, <see cref="ITimer"/> and others.</para>
    /// <para>To create an <see cref="IMetricContext"/> instance, use <see cref="MetricContext"/> implementation.</para>
    /// </summary>
    [PublicAPI]
    public interface IMetricContext
    {
        /// <summary>
        /// <para>A collection of <see cref="MetricTag"/>s associated with this <see cref="IMetricContext"/></para>
        /// <para>All metrics created with this context will include these <see cref="Tags"/>.</para>
        /// </summary>
        [NotNull]
        MetricTags Tags { get; }

        /// <summary>
        /// <para>Sends given <see cref="MetricEvent"/> for further processing.</para>
        /// <para>Use this method directly to send custom events.</para>
        /// </summary>
        void Send([NotNull] MetricEvent @event);

        /// <summary>
        /// <para>Registers an instance of <see cref="IScrapableMetric"/> for scraping.</para>
        /// <para>
        /// Once per <paramref name="scrapePeriod"/> the <see cref="IScrapableMetric.Scrape"/> method
        /// will be called on the specified <paramref name="metric"/>.
        /// </para>
        /// <para>
        /// Implementations should guarantee that <see cref="IScrapableMetric.Scrape"/> is never called concurrently on the same <paramref name="metric"/>.
        /// </para>
        /// <para>
        /// Implementations should also guarantee that <see cref="IScrapableMetric.Scrape"/> will not be invoked again after <see cref="IDisposable.Dispose"/> call.
        /// </para>
        /// </summary>
        /// <param name="metric">The metric to scrape</param>
        /// <param name="scrapePeriod">How often to scrape</param>
        /// <returns>The <see cref="IDisposable"/> token. Call <see cref="IDisposable.Dispose"/> to stop scraping the <paramref name="metric"/></returns>
        [NotNull]
        IDisposable Register([NotNull] IScrapableMetric metric, [CanBeNull] TimeSpan? scrapePeriod);
    }
}