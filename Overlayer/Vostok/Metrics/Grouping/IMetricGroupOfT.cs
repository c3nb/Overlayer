using JetBrains.Annotations;
using Vostok.Metrics.Models;

namespace Vostok.Metrics.Grouping
{
    /// <summary>
    /// <para>Represents a group of metrics of type <typeparamref name="TMetric"/>.</para>
    /// <para>Metrics in a group share the name but have different dynamic tags specified in <see cref="For"/> method.</para>
    /// </summary>
    [PublicAPI]
    public interface IMetricGroup<in TFor, out TMetric>
    {
        /// <summary>
        /// Retrieves a metric associated with specific tags from this group.
        /// </summary>
        /// <param name="value">An object to retrieve tags from. Tags are retrieved from public properties marked with <see cref="MetricTagAttribute"/>.</param>
        [NotNull]
        TMetric For([NotNull] TFor value);
    }
}