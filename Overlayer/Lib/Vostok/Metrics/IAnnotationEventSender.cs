using JetBrains.Annotations;
using Vostok.Metrics.Models;

namespace Vostok.Metrics
{
    /// <summary>
    /// Sends <see cref="AnnotationEvent"/>s to a permanent storage.
    /// Implementations are expected to be thread-safe and exception-free.
    /// </summary>
    [PublicAPI]
    public interface IAnnotationEventSender
    {
        /// <inheritdoc cref="IAnnotationEventSender"/>
        void Send([NotNull] AnnotationEvent @event);
    }
}
