using JetBrains.Annotations;

namespace Vostok.Metrics.Grouping
{
    /// <summary>
    /// <para>Represents a group of metrics of type <typeparamref name="TMetric"/>.</para>
    /// <para>Metrics in a group share the name but have different dynamic tags specified in <see cref="For"/> method.</para>
    /// </summary>
    [PublicAPI]
    public interface IMetricGroup1<out TMetric>
    {
        /// <summary>
        /// Retrieves a metric associated with specific tags from this group.
        /// </summary>
        /// <param name="value1">Value of tag number 1. Keys were defined at group construction time.</param>
        [NotNull]
        TMetric For([NotNull] string value1);
    }

    /// <summary>
    /// <para>Represents a group of metrics of type <typeparamref name="TMetric"/>.</para>
    /// <para>Metrics in a group share the name but have different dynamic tags specified in <see cref="For"/> method.</para>
    /// </summary>
    [PublicAPI]
    public interface IMetricGroup2<out TMetric>
    {
        /// <summary>
        /// Retrieves a metric associated with specific tags from this group.
        /// </summary>
        /// <param name="value1">Value of tag number 1. Keys were defined at group construction time.</param>
        /// <param name="value2">Value of tag number 2. Keys were defined at group construction time.</param>
        [NotNull]
        TMetric For([NotNull] string value1, [NotNull] string value2);
    }

    /// <summary>
    /// <para>Represents a group of metrics of type <typeparamref name="TMetric"/>.</para>
    /// <para>Metrics in a group share the name but have different dynamic tags specified in <see cref="For"/> method.</para>
    /// </summary>
    [PublicAPI]
    public interface IMetricGroup3<out TMetric>
    {
        /// <summary>
        /// Retrieves a metric associated with specific tags from this group.
        /// </summary>
        /// <param name="value1">Value of tag number 1. Keys were defined at group construction time.</param>
        /// <param name="value2">Value of tag number 2. Keys were defined at group construction time.</param>
        /// <param name="value3">Value of tag number 3. Keys were defined at group construction time.</param>
        [NotNull]
        TMetric For([NotNull] string value1, [NotNull] string value2, [NotNull] string value3);
    }

    /// <summary>
    /// <para>Represents a group of metrics of type <typeparamref name="TMetric"/>.</para>
    /// <para>Metrics in a group share the name but have different dynamic tags specified in <see cref="For"/> method.</para>
    /// </summary>
    [PublicAPI]
    public interface IMetricGroup4<out TMetric>
    {
        /// <summary>
        /// Retrieves a metric associated with specific tags from this group.
        /// </summary>
        /// <param name="value1">Value of tag number 1. Keys were defined at group construction time.</param>
        /// <param name="value2">Value of tag number 2. Keys were defined at group construction time.</param>
        /// <param name="value3">Value of tag number 3. Keys were defined at group construction time.</param>
        /// <param name="value4">Value of tag number 4. Keys were defined at group construction time.</param>
        [NotNull]
        TMetric For([NotNull] string value1, [NotNull] string value2, [NotNull] string value3, [NotNull] string value4);
    }

    /// <summary>
    /// <para>Represents a group of metrics of type <typeparamref name="TMetric"/>.</para>
    /// <para>Metrics in a group share the name but have different dynamic tags specified in <see cref="For"/> method.</para>
    /// </summary>
    [PublicAPI]
    public interface IMetricGroup5<out TMetric>
    {
        /// <summary>
        /// Retrieves a metric associated with specific tags from this group.
        /// </summary>
        /// <param name="value1">Value of tag number 1. Keys were defined at group construction time.</param>
        /// <param name="value2">Value of tag number 2. Keys were defined at group construction time.</param>
        /// <param name="value3">Value of tag number 3. Keys were defined at group construction time.</param>
        /// <param name="value4">Value of tag number 4. Keys were defined at group construction time.</param>
        /// <param name="value5">Value of tag number 5. Keys were defined at group construction time.</param>
        [NotNull]
        TMetric For([NotNull] string value1, [NotNull] string value2, [NotNull] string value3, [NotNull] string value4, [NotNull] string value5);
    }

    /// <summary>
    /// <para>Represents a group of metrics of type <typeparamref name="TMetric"/>.</para>
    /// <para>Metrics in a group share the name but have different dynamic tags specified in <see cref="For"/> method.</para>
    /// </summary>
    [PublicAPI]
    public interface IMetricGroup6<out TMetric>
    {
        /// <summary>
        /// Retrieves a metric associated with specific tags from this group.
        /// </summary>
        /// <param name="value1">Value of tag number 1. Keys were defined at group construction time.</param>
        /// <param name="value2">Value of tag number 2. Keys were defined at group construction time.</param>
        /// <param name="value3">Value of tag number 3. Keys were defined at group construction time.</param>
        /// <param name="value4">Value of tag number 4. Keys were defined at group construction time.</param>
        /// <param name="value5">Value of tag number 5. Keys were defined at group construction time.</param>
        /// <param name="value6">Value of tag number 6. Keys were defined at group construction time.</param>
        [NotNull]
        TMetric For([NotNull] string value1, [NotNull] string value2, [NotNull] string value3, [NotNull] string value4, [NotNull] string value5, [NotNull] string value6);
    }

    /// <summary>
    /// <para>Represents a group of metrics of type <typeparamref name="TMetric"/>.</para>
    /// <para>Metrics in a group share the name but have different dynamic tags specified in <see cref="For"/> method.</para>
    /// </summary>
    [PublicAPI]
    public interface IMetricGroup7<out TMetric>
    {
        /// <summary>
        /// Retrieves a metric associated with specific tags from this group.
        /// </summary>
        /// <param name="value1">Value of tag number 1. Keys were defined at group construction time.</param>
        /// <param name="value2">Value of tag number 2. Keys were defined at group construction time.</param>
        /// <param name="value3">Value of tag number 3. Keys were defined at group construction time.</param>
        /// <param name="value4">Value of tag number 4. Keys were defined at group construction time.</param>
        /// <param name="value5">Value of tag number 5. Keys were defined at group construction time.</param>
        /// <param name="value6">Value of tag number 6. Keys were defined at group construction time.</param>
        /// <param name="value7">Value of tag number 7. Keys were defined at group construction time.</param>
        [NotNull]
        TMetric For([NotNull] string value1, [NotNull] string value2, [NotNull] string value3, [NotNull] string value4, [NotNull] string value5, [NotNull] string value6, [NotNull] string value7);
    }

    /// <summary>
    /// <para>Represents a group of metrics of type <typeparamref name="TMetric"/>.</para>
    /// <para>Metrics in a group share the name but have different dynamic tags specified in <see cref="For"/> method.</para>
    /// </summary>
    [PublicAPI]
    public interface IMetricGroup8<out TMetric>
    {
        /// <summary>
        /// Retrieves a metric associated with specific tags from this group.
        /// </summary>
        /// <param name="value1">Value of tag number 1. Keys were defined at group construction time.</param>
        /// <param name="value2">Value of tag number 2. Keys were defined at group construction time.</param>
        /// <param name="value3">Value of tag number 3. Keys were defined at group construction time.</param>
        /// <param name="value4">Value of tag number 4. Keys were defined at group construction time.</param>
        /// <param name="value5">Value of tag number 5. Keys were defined at group construction time.</param>
        /// <param name="value6">Value of tag number 6. Keys were defined at group construction time.</param>
        /// <param name="value7">Value of tag number 7. Keys were defined at group construction time.</param>
        /// <param name="value8">Value of tag number 8. Keys were defined at group construction time.</param>
        [NotNull]
        TMetric For([NotNull] string value1, [NotNull] string value2, [NotNull] string value3, [NotNull] string value4, [NotNull] string value5, [NotNull] string value6, [NotNull] string value7, [NotNull] string value8);
    }
}