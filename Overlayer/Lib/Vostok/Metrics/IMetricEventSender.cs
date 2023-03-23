using JetBrains.Annotations;
using Vostok.Metrics.Models;

namespace Vostok.Metrics
{
    /// <summary>
    /// Sends <see cref="MetricEvent"/>s for further aggregation or to a permanent storage.
    /// Implementations are expected to be thread-safe and exception-free.
    /// </summary>
    [PublicAPI]
    public interface IMetricEventSender
    {
        /// <inheritdoc cref="IMetricEventSender"/>
        void Send([NotNull] MetricEvent @event);
    }
}