using JetBrains.Annotations;
using Vostok.Metrics.Models;

namespace Vostok.Metrics
{
    [PublicAPI]
    public static class IMetricContextExtensions_Sending
    {
        /// <summary>
        /// Converts given <see cref="MetricDataPoint"/> to a <see cref="MetricEvent"/> using given <paramref name="context"/>'s tags and sends it for further processing.
        /// </summary>
        public static void Send([NotNull] this IMetricContext context, [NotNull] MetricDataPoint point)
            => context.Send(point.ToMetricEvent(context.Tags));

        /// <summary>
        /// <para>Converts given <paramref name="description"/> and <paramref name="tags"/> to an <see cref="AnnotationEvent"/> using given <paramref name="context"/>'s tags and sends it for further processing.</para>
        /// <para>Fails if <paramref name="context"/> does not implement <see cref="IAnnotationContext"/>.</para>
        /// </summary>
        public static void SendAnnotation(
            [NotNull] this IMetricContext context,
            [NotNull] string description,
            [NotNull] params (string key, string value)[] tags)
            => context.AsAnnotationContext().Send(description, tags);
    }
}