using JetBrains.Annotations;
using Vostok.Metrics.Models;

namespace Vostok.Metrics.Senders
{
    /// <summary>
    /// Drops all incoming <see cref="MetricEvent"/>s.
    /// </summary>
    [PublicAPI]
    public class DevNullMetricEventSender : IMetricEventSender
    {
        public void Send(MetricEvent @event)
        {
        }
    }
}