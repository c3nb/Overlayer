using System;
using System.Collections.Generic;
using Vostok.Metrics.Models;

namespace Vostok.Metrics.Senders
{
    internal class CompositeDynamicAnnotationEventSender : IAnnotationEventSender
    {
        private readonly IAnnotationEventSender primarySender;
        private readonly Func<IEnumerable<IAnnotationEventSender>> additionalSenders;

        public CompositeDynamicAnnotationEventSender(IAnnotationEventSender primarySender, Func<IEnumerable<IAnnotationEventSender>> additionalSenders)
        {
            this.primarySender = primarySender;
            this.additionalSenders = additionalSenders;
        }

        public void Send(AnnotationEvent @event)
        {
            foreach (var sender in EnumerateSenders())
                sender.Send(@event);
        }

        private IEnumerable<IAnnotationEventSender> EnumerateSenders()
        {
            yield return primarySender;

            foreach (var additionalSender in additionalSenders())
                yield return additionalSender;
        }
    }
}
