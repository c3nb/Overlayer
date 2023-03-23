using System.Collections.Generic;
using JetBrains.Annotations;

namespace Vostok.Metrics.Models
{
    [PublicAPI]
    public static class MetricTagsMerger
    {
        /// <summary>
        /// <para>Appends a <see cref="WellKnownTagKeys.Name"/> tag with value taken from given <paramref name="name"/> parameter to given <paramref name="contextTags"/>.</para>
        /// <para>Then appends <paramref name="dynamicTags"/> after <see cref="WellKnownTagKeys.Name"/> and returns the result.</para>
        /// </summary>
        [NotNull]
        public static MetricTags Merge([NotNull] MetricTags contextTags, [CanBeNull] string name, [NotNull] IReadOnlyList<MetricTag> dynamicTags)
        {
            var result = contextTags;

            if (!string.IsNullOrEmpty(name))
                result = result.Append(WellKnownTagKeys.Name, name);

            return result.Append(dynamicTags);
        }

        /// <summary>
        /// Appends a <see cref="WellKnownTagKeys.Name"/> tag with value taken from given <paramref name="name"/> parameter to given <paramref name="contextTags"/> and returns resulting tags.
        /// </summary>
        [NotNull]
        public static MetricTags Merge([NotNull] MetricTags contextTags, [NotNull] string name)
            => contextTags.Append(WellKnownTagKeys.Name, name);
    }
}