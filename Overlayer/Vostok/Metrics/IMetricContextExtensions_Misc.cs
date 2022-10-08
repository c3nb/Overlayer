using System;
using JetBrains.Annotations;
using Vostok.Metrics.Scraping;

namespace Vostok.Metrics
{
    internal static class IMetricContextExtensions_Misc
    {
        [NotNull]
        public static IAnnotationContext AsAnnotationContext([NotNull] this IMetricContext context)
            => context as IAnnotationContext ?? throw new NotSupportedException($"This metric context of type '{context.GetType().Name}' does not support annotations.");

        public static IDisposable Register(this IMetricContext context, IScrapableMetric metric, ScrapableMetricConfig config)
        {
            if (context is IScrapeConfigurableMetricContext scrapeConfigurableMetricContext)
                return scrapeConfigurableMetricContext.Register(metric, config);

            return context.Register(metric, config.ScrapePeriod);
        }
    }
}