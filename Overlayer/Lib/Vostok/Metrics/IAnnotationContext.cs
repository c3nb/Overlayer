using JetBrains.Annotations;
using Vostok.Metrics.Models;

namespace Vostok.Metrics
{
    [PublicAPI]
    public interface IAnnotationContext
    {
        /// <summary>
        /// <para>A collection of <see cref="MetricTag"/>s associated with this <see cref="IAnnotationContext"/></para>
        /// <para>All annotations created with this context will include these <see cref="Tags"/>.</para>
        /// </summary>
        [NotNull]
        MetricTags Tags { get; }

        /// <summary>
        /// <para>Sends given <see cref="AnnotationEvent"/> for further processing.</para>
        /// <para>Use this method directly to send events with complete tags.</para>
        /// </summary>
        void Send([NotNull] AnnotationEvent @event);
    }
}
