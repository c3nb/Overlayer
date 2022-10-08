using System;
using JetBrains.Annotations;
using Vostok.Metrics.Models;

namespace Vostok.Metrics.Senders
{
    /// <summary>
    /// This implementation of <see cref="IMetricEventSender"/> forwards incoming events to an arbitrary number of underlying senders.
    /// </summary>
    [PublicAPI]
    public class CompositeMetricEventSender : IMetricEventSender
    {
        private readonly IMetricEventSender[] senders;

        public CompositeMetricEventSender([NotNull] IMetricEventSender[] senders)
            => this.senders = senders ?? throw new ArgumentNullException(nameof(senders));

        public void Send(MetricEvent @event)
        {
            foreach (var sender in senders)
            {
                sender.Send(@event);
            }
        }
    }
}