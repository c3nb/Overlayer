using System;
using System.Linq;
using JetBrains.Annotations;
using Vostok.Metrics.Models;

namespace Vostok.Metrics
{
    [PublicAPI]
    public static class IAnnotationContextExtensions
    {
        /// <summary>
        /// Converts given <paramref name="description"/> and <paramref name="tags"/> to an <see cref="AnnotationEvent"/> using given <paramref name="context"/>'s tags and sends it for further processing.
        /// </summary>
        public static void Send(
            [NotNull] this IAnnotationContext context,
            [NotNull] string description,
            [NotNull] params (string key, string value)[] tags)
            => context.Send(CreateAnnotationEvent(context.Tags, tags, description));

        private static AnnotationEvent CreateAnnotationEvent(
            [NotNull] MetricTags contextTags,
            [NotNull] (string key, string value)[] customTags,
            [NotNull] string description)
        {
            var eventTags = contextTags.Append(customTags.Select(pair => new MetricTag(pair.key, pair.value)).ToArray());

            return new AnnotationEvent(DateTimeOffset.Now, eventTags, description);
        }
    }
}
