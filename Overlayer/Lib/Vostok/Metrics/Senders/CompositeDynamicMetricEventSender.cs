using System;
using System.Collections.Generic;
using Vostok.Metrics.Models;

namespace Vostok.Metrics.Senders
{
    internal class CompositeDynamicMetricEventSender : IMetricEventSender
    {
        private readonly IMetricEventSender primarySender;
        private readonly Func<IEnumerable<IMetricEventSender>> additionalSenders;

        public CompositeDynamicMetricEventSender(IMetricEventSender primarySender, Func<IEnumerable<IMetricEventSender>> additionalSenders)
        {
            this.primarySender = primarySender;
            this.additionalSenders = additionalSenders;
        }

        public void Send(MetricEvent @event)
        {
            foreach (var sender in EnumerateSenders())
                sender.Send(@event);
        }

        private IEnumerable<IMetricEventSender> EnumerateSenders()
        {
            yield return primarySender;

            foreach (var additionalSender in additionalSenders())
                yield return additionalSender;
        }
    }
}
