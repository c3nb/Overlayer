using System;
using JetBrains.Annotations;
using Vostok.Metrics.Models;

namespace Vostok.Metrics.Senders
{
    /// <summary>
    /// Delegates <see cref="Send"/> implementation to a custom action.
    /// </summary>
    [PublicAPI]
    public class AdHocMetricEventSender : IMetricEventSender
    {
        private readonly Action<MetricEvent> sendAction;

        /// <param name="sendAction">
        /// This will be called every time <see cref="Send"/> occurs.
        /// The delegate should be thread-safe and exception-free.
        /// </param>
        public AdHocMetricEventSender([NotNull] Action<MetricEvent> sendAction)
            => this.sendAction = sendAction ?? throw new ArgumentNullException(nameof(sendAction));

        public void Send(MetricEvent @event)
            => sendAction(@event);
    }
}