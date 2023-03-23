using System;
using JetBrains.Annotations;
using Vostok.Metrics.Models;
using Vostok.Metrics.Scraping;

namespace Vostok.Metrics
{
    [PublicAPI]
    public static class IMetricContextExtensions_Tags
    {
        /// <summary>
        /// Creates a child <see cref="IMetricContext"/> with additional tags appended to current <paramref name="context"/> tags.
        /// </summary>
        [NotNull]
        public static IMetricContext WithTag([NotNull] this IMetricContext context, [NotNull] string key, [NotNull] string value)
            => context.OverwriteTags(context.Tags.Append(key, value));

        /// <inheritdoc cref="WithTag(IMetricContext,string,string)"/>
        [NotNull]
        public static IMetricContext WithTag([NotNull] this IMetricContext context, [NotNull] MetricTag tag)
            => context.OverwriteTags(context.Tags.Append(tag));

        /// <inheritdoc cref="WithTag(IMetricContext,string,string)"/>
        [NotNull]
        public static IMetricContext WithTags([NotNull] this IMetricContext context, [NotNull] MetricTags tags)
            => context.OverwriteTags(context.Tags.Append(tags));

        /// <summary>
        /// Creates a new <see cref="IMetricContext"/> with <see cref="IMetricContext.Tags"/> set to given <paramref name="newTags"/>, but otherwise delegating to given <paramref name="context"/>.
        /// </summary>
        [NotNull]
        public static IMetricContext OverwriteTags([NotNull] this IMetricContext context, [CanBeNull] MetricTags newTags)
            => new OverwriteTagsWrapper(context, newTags);

        private class OverwriteTagsWrapper : IMetricContext, IAnnotationContext, IMetricContextWrapper, IScrapeConfigurableMetricContext
        {
            public OverwriteTagsWrapper(IMetricContext context, MetricTags tags)
            {
                BaseContext = context;
                Tags = tags ?? MetricTags.Empty;
            }

            public IMetricContext BaseContext { get; }

            public MetricTags Tags { get; }

            public IDisposable Register(IScrapableMetric metric, TimeSpan? scrapePeriod)
                => BaseContext.Register(metric, scrapePeriod);

            public void Send(MetricEvent @event)
                => BaseContext.Send(@event);

            public void Send(AnnotationEvent @event)
                => BaseContext.AsAnnotationContext().Send(@event);

            public IDisposable Register(IScrapableMetric metric, ScrapableMetricConfig scrapableMetricConfig) =>
                BaseContext.Register(metric, scrapableMetricConfig);
        }
    }
}