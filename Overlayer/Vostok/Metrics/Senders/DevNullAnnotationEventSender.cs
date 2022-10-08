using JetBrains.Annotations;
using Vostok.Metrics.Models;

namespace Vostok.Metrics.Senders
{
    /// <summary>
    /// Drops all incoming <see cref="AnnotationEvent"/>s.
    /// </summary>
    [PublicAPI]
    public class DevNullAnnotationEventSender : IAnnotationEventSender
    {
        public void Send(AnnotationEvent @event)
        {
        }
    }
}
