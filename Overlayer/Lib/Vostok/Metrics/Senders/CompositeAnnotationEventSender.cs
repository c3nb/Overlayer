using System;
using JetBrains.Annotations;
using Vostok.Metrics.Models;

namespace Vostok.Metrics.Senders
{
    /// <summary>
    /// This implementation of <see cref="IAnnotationEventSender"/> forwards incoming events to an arbitrary number of underlying senders.
    /// </summary>
    [PublicAPI]
    public class CompositeAnnotationEventSender : IAnnotationEventSender
    {
        private readonly IAnnotationEventSender[] senders;

        public CompositeAnnotationEventSender([NotNull] IAnnotationEventSender[] senders)
            => this.senders = senders ?? throw new ArgumentNullException(nameof(senders));

        public void Send(AnnotationEvent @event)
        {
            foreach (var sender in senders)
            {
                sender.Send(@event);
            }
        }
    }
}
