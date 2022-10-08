using System;
using System.Threading;
using JetBrains.Annotations;
using Vostok.Metrics.Models;
using Vostok.Metrics.Primitives.Caching;
using Vostok.Metrics.Primitives.Counter;
using Vostok.Metrics.Scraping;
using Vostok.Metrics.Senders;

// ReSharper disable SuspiciousTypeConversion.Global

namespace Vostok.Metrics
{
    /// <inheritdoc cref="IMetricContext"/>
    [PublicAPI]
    public class MetricContext : IMetricContext, IAnnotationContext, IDisposable, IScrapeConfigurableMetricContext
    {
        private static IMetricEventSender[] globalMetricSenders = Array.Empty<IMetricEventSender>();
        private static IAnnotationEventSender[] globalAnnotationSenders = Array.Empty<IAnnotationEventSender>();

        private readonly MetricContextConfig config;
        private readonly IMetricEventSender metricSender;
        private readonly IAnnotationEventSender annotationSender;

        private readonly ScrapeScheduler scheduler;
        private readonly ScrapeScheduler fastScheduler;
        private readonly ScrapeScheduler scrapeOnDisposeScheduler;

        public MetricContext([NotNull] MetricContextConfig config)
        {
            this.config = config ?? throw new ArgumentNullException(nameof(config));

            metricSender = new CompositeDynamicMetricEventSender(config.Sender, () => globalMetricSenders);

            annotationSender = new CompositeDynamicAnnotationEventSender(
                config.AnnotationSender ?? config.Sender as IAnnotationEventSender ?? new DevNullAnnotationEventSender(), () => globalAnnotationSenders);

            scheduler = new ScrapeScheduler(metricSender, config.ErrorCallback);
            fastScheduler = new ScrapeScheduler(metricSender, config.ErrorCallback);
            scrapeOnDisposeScheduler = new ScrapeScheduler(metricSender, config.ErrorCallback, true);
        }

        public static void AddGlobalSender([NotNull] IMetricEventSender sender)
            => AddGlobalSender(sender, ref globalMetricSenders);

        public static void AddGlobalSender([NotNull] IAnnotationEventSender sender)
            => AddGlobalSender(sender, ref globalAnnotationSenders);

        public MetricTags Tags => config.Tags ?? MetricTags.Empty;

        public IDisposable Register(IScrapableMetric metric, TimeSpan? scrapePeriod) =>
            GetScheduler(metric, ShouldBeScrapedOnDisposeByDefault(metric))
               .Register(metric, scrapePeriod ?? config.DefaultScrapePeriod);

        public void Send(MetricEvent @event)
            => metricSender.Send(@event);

        IDisposable IScrapeConfigurableMetricContext.Register(IScrapableMetric metric, ScrapableMetricConfig scrapableMetricConfig) =>
            GetScheduler(metric, scrapableMetricConfig?.ScrapeOnDispose ?? ShouldBeScrapedOnDisposeByDefault(metric))
               .Register(metric, scrapableMetricConfig?.ScrapePeriod ?? config.DefaultScrapePeriod);

        public void Send(AnnotationEvent @event)
            => annotationSender.Send(@event);

        public void Dispose()
        {
            scheduler.Dispose();
            fastScheduler.Dispose();
            scrapeOnDisposeScheduler.Dispose();
            GlobalCache.Clean(this);
        }

        private static void AddGlobalSender<TSender>([NotNull] TSender sender, ref TSender[] globalSenders)
        {
            if (sender == null)
                throw new ArgumentNullException(nameof(sender));

            var oldGlobalSenders = globalSenders;
            var newGlobalSenders = new TSender[oldGlobalSenders.Length + 1];

            Array.Copy(oldGlobalSenders, newGlobalSenders, oldGlobalSenders.Length);

            newGlobalSenders[oldGlobalSenders.Length] = sender;

            Interlocked.Exchange(ref globalSenders, newGlobalSenders);
        }

        private ScrapeScheduler GetScheduler(IScrapableMetric metric, bool scrapeOnDispose)
        {
            if (scrapeOnDispose)
                return scrapeOnDisposeScheduler;

            if (metric is IFastScrapableMetric)
                return fastScheduler;

            return scheduler;
        }

        private bool ShouldBeScrapedOnDisposeByDefault(IScrapableMetric metric) => metric is ICounter;
    }
}
