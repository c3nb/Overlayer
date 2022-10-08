using System;
using JetBrains.Annotations;
using Vostok.Metrics.Models;
using Vostok.Metrics.Scraping;

namespace Vostok.Metrics
{
    /// <summary>
    /// A trivial implementation of <see cref="IMetricContext"/> that simply does nothing.
    /// </summary>
    [PublicAPI]
    public class DevNullMetricContext : IMetricContext, IAnnotationContext
    {
        public MetricTags Tags => MetricTags.Empty;

        public IDisposable Register(IScrapableMetric metric, TimeSpan? scrapePeriod)
            => new DummyRegistrationToken();

        public void Send(MetricEvent @event)
        {
        }

        public void Send(AnnotationEvent @event)
        {
        }

        private class DummyRegistrationToken : IDisposable
        {
            public void Dispose()
            {
            }
        }
    }
}