using System;
using JetBrains.Annotations;
using Vostok.Metrics.Models;

namespace Vostok.Metrics
{
    /// <summary>
    /// <para>Represents configuration of a <see cref="MetricContext"/> instance.</para>
    /// <para></para>
    /// </summary>
    [PublicAPI]
    public class MetricContextConfig
    {
        public MetricContextConfig([NotNull] IMetricEventSender sender)
            => Sender = sender ?? throw new ArgumentNullException(nameof(sender));

        /// <summary>
        /// Sender used to offload <see cref="MetricEvent"/>s for further processing.
        /// </summary>
        [NotNull]
        public IMetricEventSender Sender { get; }

        /// <summary>
        /// Sender used to offload <see cref="AnnotationEvent"/>s for further processing.
        /// </summary>
        [CanBeNull]
        public IAnnotationEventSender AnnotationSender { get; set; }

        /// <summary>
        /// A set of tags inherent to every metric produced with configured context instance.
        /// </summary>
        [CanBeNull]
        public MetricTags Tags { get; set; }

        /// <summary>
        /// An optional callback invoked when an internal exception occurs (such as an error while scraping a metric).
        /// </summary>
        [CanBeNull]
        public Action<Exception> ErrorCallback { get; set; }

        /// <summary>
        /// Default metric scrape period (used when passing a <c>null</c> period to <see cref="IMetricContext.Register"/> method).
        /// </summary>
        public TimeSpan DefaultScrapePeriod { get; set; } = TimeSpan.FromMinutes(1);
    }
}