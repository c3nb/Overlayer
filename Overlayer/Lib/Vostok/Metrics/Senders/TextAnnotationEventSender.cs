using System;
using JetBrains.Annotations;
using Vostok.Metrics.Models;

namespace Vostok.Metrics.Senders
{
    /// <summary>
    /// Renders incoming <see cref="AnnotationEvent"/>s to text and executes an arbitrary delegate on them.
    /// </summary>
    [PublicAPI]
    public class TextAnnotationEventSender : IAnnotationEventSender
    {
        private readonly Action<string> send;

        public TextAnnotationEventSender([NotNull] Action<string> send)
            => this.send = send ?? throw new ArgumentNullException(nameof(send));

        public void Send(AnnotationEvent @event)
            => send(EventPrinter.Print(@event));
    }
}
