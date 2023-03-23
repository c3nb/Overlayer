using System;
using JetBrains.Annotations;
using Vostok.Metrics.Models;

namespace Vostok.Metrics.Senders
{
    /// <summary>
    /// Delegates <see cref="Send"/> implementation to a custom action.
    /// </summary>
    [PublicAPI]
    public class AdHocAnnotationEventSender : IAnnotationEventSender
    {
        private readonly Action<AnnotationEvent> sendAction;

        /// <param name="sendAction">
        /// This will be called every time <see cref="Send"/> occurs.
        /// The delegate should be thread-safe and exception-free.
        /// </param>
        public AdHocAnnotationEventSender([NotNull] Action<AnnotationEvent> sendAction)
            => this.sendAction = sendAction ?? throw new ArgumentNullException(nameof(sendAction));

        public void Send(AnnotationEvent @event)
            => sendAction(@event);
    }
}
