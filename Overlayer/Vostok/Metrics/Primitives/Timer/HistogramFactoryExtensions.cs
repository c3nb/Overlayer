using System;
using JetBrains.Annotations;
using Vostok.Metrics.Grouping;
using Vostok.Metrics.Models;
using Vostok.Metrics.Primitives.Caching;

namespace Vostok.Metrics.Primitives.Timer
{
    [PublicAPI]
    public static class HistogramFactoryExtensions
    {
        /// <inheritdoc cref="Vostok.Metrics.Primitives.Timer.Histogram"/>
        /// <param name="context">Context this metric will belong to.</param>
        /// <param name="name">Name of the metric. It will be added to event's <see cref="MetricEvent.Tags"/> with key set to <see cref="Vostok.Metrics.WellKnownTagKeys.Name"/>.</param>
        /// <param name="config">Optional metric-specific config.</param>
        [NotNull]
        public static ITimer CreateHistogram([NotNull] this IMetricContext context, [NotNull] string name, [CanBeNull] HistogramConfig config = null)
            => GlobalCache.Obtain(context, name, null, () => new Histogram(context, MetricTagsMerger.Merge(context.Tags, name), config ?? HistogramConfig.Default));

        #region Metric group extensions

        /// <summary>
        /// <para>
        /// Creates a group of <see cref="Vostok.Metrics.Primitives.Timer.Histogram">Histograms</see>.
        /// Metrics in the group share the same context tags and <paramref name="name"/> but have different dynamic tags.
        /// </para>
        /// <para>Dynamic tags are specified by an instance of <typeparamref name="TFor"/>.</para>
        /// <para><typeparamref name="TFor"/> type must have at least one public property marked with <see cref="MetricTagAttribute"/>.</para>
        /// <inheritdoc cref="Vostok.Metrics.Primitives.Timer.Histogram"/>
        /// </summary>
        /// <param name="context">Context this metric will belong to.</param>
        /// <param name="name">Name of the metric. It will be added to event's <see cref="MetricEvent.Tags"/> with key set to <see cref="Vostok.Metrics.WellKnownTagKeys.Name"/>.</param>
        /// <param name="config">Optional metric-specific config.</param>
        /// <inheritdoc cref="Vostok.Metrics.Primitives.Timer.Histogram"/>
        [NotNull]
        public static IMetricGroup<TFor, ITimer> CreateHistogram<TFor>([NotNull] this IMetricContext context, [NotNull] string name, [CanBeNull] HistogramConfig config = null)
            => GlobalCache.Obtain(context, name, typeof(TFor), () => new MetricGroup<TFor, ITimer>(MetricForTagsFactory(context, name, config ?? HistogramConfig.Default)));

        /// <summary>
        /// <para>
        /// Creates a group of <see cref="Vostok.Metrics.Primitives.Timer.Histogram">Histograms</see>.
        /// Metrics in the group share the same context tags and <paramref name="name"/> but have different dynamic tags.
        /// </para>
        /// <para>
        /// Dynamic tags are specified by string parameters. Define the keys now and pass the values later.
        /// </para>
        /// <inheritdoc cref="Vostok.Metrics.Primitives.Timer.Histogram"/>
        /// </summary>
        /// <param name="context">Context this metric will belong to.</param>
        /// <param name="name">Name of the metric. It will be added to event's <see cref="MetricEvent.Tags"/> with key set to <see cref="Vostok.Metrics.WellKnownTagKeys.Name"/>.</param>
        /// <param name="key1">Key of dynamic tag number 1.</param>
        /// <param name="config">Optional config</param>
        /// <inheritdoc cref="Vostok.Metrics.Primitives.Timer.Histogram"/>
        [NotNull]
        public static IMetricGroup1<ITimer> CreateHistogram([NotNull] this IMetricContext context, [NotNull] string name, [NotNull] string key1, [CanBeNull] HistogramConfig config = null)
            => GlobalCache.Obtain(context, name, key1, () => CreateMetricGroup(context, name, config, key1));

        /// <summary>
        /// <para>
        /// Creates a group of <see cref="Vostok.Metrics.Primitives.Timer.Histogram">Histograms</see>.
        /// Metrics in the group share the same context tags and <paramref name="name"/> but have different dynamic tags.
        /// </para>
        /// <para>
        /// Dynamic tags are specified by string parameters. Define the keys now and pass the values later.
        /// </para>
        /// <inheritdoc cref="Vostok.Metrics.Primitives.Timer.Histogram"/>
        /// </summary>
        /// <param name="context">Context this metric will belong to.</param>
        /// <param name="name">Name of the metric. It will be added to event's <see cref="MetricEvent.Tags"/> with key set to <see cref="Vostok.Metrics.WellKnownTagKeys.Name"/>.</param>
        /// <param name="key1">Key of dynamic tag number 1.</param>
        /// <param name="key2">Key of dynamic tag number 2.</param>
        /// <param name="config">Optional config</param>
        /// <inheritdoc cref="Vostok.Metrics.Primitives.Timer.Histogram"/>
        [NotNull]
        public static IMetricGroup2<ITimer> CreateHistogram([NotNull] this IMetricContext context, [NotNull] string name, [NotNull] string key1, [NotNull] string key2, [CanBeNull] HistogramConfig config = null)
            => GlobalCache.Obtain(context, name, (key1, key2), () => CreateMetricGroup(context, name, config, key1, key2));

        /// <summary>
        /// <para>
        /// Creates a group of <see cref="Vostok.Metrics.Primitives.Timer.Histogram">Histograms</see>.
        /// Metrics in the group share the same context tags and <paramref name="name"/> but have different dynamic tags.
        /// </para>
        /// <para>
        /// Dynamic tags are specified by string parameters. Define the keys now and pass the values later.
        /// </para>
        /// <inheritdoc cref="Vostok.Metrics.Primitives.Timer.Histogram"/>
        /// </summary>
        /// <param name="context">Context this metric will belong to.</param>
        /// <param name="name">Name of the metric. It will be added to event's <see cref="MetricEvent.Tags"/> with key set to <see cref="Vostok.Metrics.WellKnownTagKeys.Name"/>.</param>
        /// <param name="key1">Key of dynamic tag number 1.</param>
        /// <param name="key2">Key of dynamic tag number 2.</param>
        /// <param name="key3">Key of dynamic tag number 3.</param>
        /// <param name="config">Optional config</param>
        /// <inheritdoc cref="Vostok.Metrics.Primitives.Timer.Histogram"/>
        [NotNull]
        public static IMetricGroup3<ITimer> CreateHistogram([NotNull] this IMetricContext context, [NotNull] string name, [NotNull] string key1, [NotNull] string key2, [NotNull] string key3, [CanBeNull] HistogramConfig config = null)
            => GlobalCache.Obtain(context, name, (key1, key2, key3), () => CreateMetricGroup(context, name, config, key1, key2, key3));

        /// <summary>
        /// <para>
        /// Creates a group of <see cref="Vostok.Metrics.Primitives.Timer.Histogram">Histograms</see>.
        /// Metrics in the group share the same context tags and <paramref name="name"/> but have different dynamic tags.
        /// </para>
        /// <para>
        /// Dynamic tags are specified by string parameters. Define the keys now and pass the values later.
        /// </para>
        /// <inheritdoc cref="Vostok.Metrics.Primitives.Timer.Histogram"/>
        /// </summary>
        /// <param name="context">Context this metric will belong to.</param>
        /// <param name="name">Name of the metric. It will be added to event's <see cref="MetricEvent.Tags"/> with key set to <see cref="Vostok.Metrics.WellKnownTagKeys.Name"/>.</param>
        /// <param name="key1">Key of dynamic tag number 1.</param>
        /// <param name="key2">Key of dynamic tag number 2.</param>
        /// <param name="key3">Key of dynamic tag number 3.</param>
        /// <param name="key4">Key of dynamic tag number 4.</param>
        /// <param name="config">Optional config</param>
        /// <inheritdoc cref="Vostok.Metrics.Primitives.Timer.Histogram"/>
        [NotNull]
        public static IMetricGroup4<ITimer> CreateHistogram([NotNull] this IMetricContext context, [NotNull] string name, [NotNull] string key1, [NotNull] string key2, [NotNull] string key3, [NotNull] string key4, [CanBeNull] HistogramConfig config = null)
            => GlobalCache.Obtain(context, name, (key1, key2, key3, key4), () => CreateMetricGroup(context, name, config, key1, key2, key3, key4));

        /// <summary>
        /// <para>
        /// Creates a group of <see cref="Vostok.Metrics.Primitives.Timer.Histogram">Histograms</see>.
        /// Metrics in the group share the same context tags and <paramref name="name"/> but have different dynamic tags.
        /// </para>
        /// <para>
        /// Dynamic tags are specified by string parameters. Define the keys now and pass the values later.
        /// </para>
        /// <inheritdoc cref="Vostok.Metrics.Primitives.Timer.Histogram"/>
        /// </summary>
        /// <param name="context">Context this metric will belong to.</param>
        /// <param name="name">Name of the metric. It will be added to event's <see cref="MetricEvent.Tags"/> with key set to <see cref="Vostok.Metrics.WellKnownTagKeys.Name"/>.</param>
        /// <param name="key1">Key of dynamic tag number 1.</param>
        /// <param name="key2">Key of dynamic tag number 2.</param>
        /// <param name="key3">Key of dynamic tag number 3.</param>
        /// <param name="key4">Key of dynamic tag number 4.</param>
        /// <param name="key5">Key of dynamic tag number 5.</param>
        /// <param name="config">Optional config</param>
        /// <inheritdoc cref="Vostok.Metrics.Primitives.Timer.Histogram"/>
        [NotNull]
        public static IMetricGroup5<ITimer> CreateHistogram([NotNull] this IMetricContext context, [NotNull] string name, [NotNull] string key1, [NotNull] string key2, [NotNull] string key3, [NotNull] string key4, [NotNull] string key5, [CanBeNull] HistogramConfig config = null)
            => GlobalCache.Obtain(context, name, (key1, key2, key3, key4, key5), () => CreateMetricGroup(context, name, config, key1, key2, key3, key4, key5));

        /// <summary>
        /// <para>
        /// Creates a group of <see cref="Vostok.Metrics.Primitives.Timer.Histogram">Histograms</see>.
        /// Metrics in the group share the same context tags and <paramref name="name"/> but have different dynamic tags.
        /// </para>
        /// <para>
        /// Dynamic tags are specified by string parameters. Define the keys now and pass the values later.
        /// </para>
        /// <inheritdoc cref="Vostok.Metrics.Primitives.Timer.Histogram"/>
        /// </summary>
        /// <param name="context">Context this metric will belong to.</param>
        /// <param name="name">Name of the metric. It will be added to event's <see cref="MetricEvent.Tags"/> with key set to <see cref="Vostok.Metrics.WellKnownTagKeys.Name"/>.</param>
        /// <param name="key1">Key of dynamic tag number 1.</param>
        /// <param name="key2">Key of dynamic tag number 2.</param>
        /// <param name="key3">Key of dynamic tag number 3.</param>
        /// <param name="key4">Key of dynamic tag number 4.</param>
        /// <param name="key5">Key of dynamic tag number 5.</param>
        /// <param name="key6">Key of dynamic tag number 6.</param>
        /// <param name="config">Optional config</param>
        /// <inheritdoc cref="Vostok.Metrics.Primitives.Timer.Histogram"/>
        [NotNull]
        public static IMetricGroup6<ITimer> CreateHistogram([NotNull] this IMetricContext context, [NotNull] string name, [NotNull] string key1, [NotNull] string key2, [NotNull] string key3, [NotNull] string key4, [NotNull] string key5, [NotNull] string key6, [CanBeNull] HistogramConfig config = null)
            => GlobalCache.Obtain(context, name, (key1, key2, key3, key4, key5, key6), () => CreateMetricGroup(context, name, config, key1, key2, key3, key4, key5, key6));

        /// <summary>
        /// <para>
        /// Creates a group of <see cref="Vostok.Metrics.Primitives.Timer.Histogram">Histograms</see>.
        /// Metrics in the group share the same context tags and <paramref name="name"/> but have different dynamic tags.
        /// </para>
        /// <para>
        /// Dynamic tags are specified by string parameters. Define the keys now and pass the values later.
        /// </para>
        /// <inheritdoc cref="Vostok.Metrics.Primitives.Timer.Histogram"/>
        /// </summary>
        /// <param name="context">Context this metric will belong to.</param>
        /// <param name="name">Name of the metric. It will be added to event's <see cref="MetricEvent.Tags"/> with key set to <see cref="Vostok.Metrics.WellKnownTagKeys.Name"/>.</param>
        /// <param name="key1">Key of dynamic tag number 1.</param>
        /// <param name="key2">Key of dynamic tag number 2.</param>
        /// <param name="key3">Key of dynamic tag number 3.</param>
        /// <param name="key4">Key of dynamic tag number 4.</param>
        /// <param name="key5">Key of dynamic tag number 5.</param>
        /// <param name="key6">Key of dynamic tag number 6.</param>
        /// <param name="key7">Key of dynamic tag number 7.</param>
        /// <param name="config">Optional config</param>
        /// <inheritdoc cref="Vostok.Metrics.Primitives.Timer.Histogram"/>
        [NotNull]
        public static IMetricGroup7<ITimer> CreateHistogram([NotNull] this IMetricContext context, [NotNull] string name, [NotNull] string key1, [NotNull] string key2, [NotNull] string key3, [NotNull] string key4, [NotNull] string key5, [NotNull] string key6, [NotNull] string key7, [CanBeNull] HistogramConfig config = null)
            => GlobalCache.Obtain(context, name, (key1, key2, key3, key4, key5, key6, key7), () => CreateMetricGroup(context, name, config, key1, key2, key3, key4, key5, key6, key7));

        /// <summary>
        /// <para>
        /// Creates a group of <see cref="Vostok.Metrics.Primitives.Timer.Histogram">Histograms</see>.
        /// Metrics in the group share the same context tags and <paramref name="name"/> but have different dynamic tags.
        /// </para>
        /// <para>
        /// Dynamic tags are specified by string parameters. Define the keys now and pass the values later.
        /// </para>
        /// <inheritdoc cref="Vostok.Metrics.Primitives.Timer.Histogram"/>
        /// </summary>
        /// <param name="context">Context this metric will belong to.</param>
        /// <param name="name">Name of the metric. It will be added to event's <see cref="MetricEvent.Tags"/> with key set to <see cref="Vostok.Metrics.WellKnownTagKeys.Name"/>.</param>
        /// <param name="key1">Key of dynamic tag number 1.</param>
        /// <param name="key2">Key of dynamic tag number 2.</param>
        /// <param name="key3">Key of dynamic tag number 3.</param>
        /// <param name="key4">Key of dynamic tag number 4.</param>
        /// <param name="key5">Key of dynamic tag number 5.</param>
        /// <param name="key6">Key of dynamic tag number 6.</param>
        /// <param name="key7">Key of dynamic tag number 7.</param>
        /// <param name="key8">Key of dynamic tag number 8.</param>
        /// <param name="config">Optional config</param>
        /// <inheritdoc cref="Vostok.Metrics.Primitives.Timer.Histogram"/>
        [NotNull]
        public static IMetricGroup8<ITimer> CreateHistogram([NotNull] this IMetricContext context, [NotNull] string name, [NotNull] string key1, [NotNull] string key2, [NotNull] string key3, [NotNull] string key4, [NotNull] string key5, [NotNull] string key6, [NotNull] string key7, [NotNull] string key8, [CanBeNull] HistogramConfig config = null)
            => GlobalCache.Obtain(context, name, (key1, key2, key3, key4, key5, key6, key7, key8), () => CreateMetricGroup(context, name, config, key1, key2, key3, key4, key5, key6, key7, key8));

        #endregion

        #region Helper methods

        private static MetricGroup<Histogram> CreateMetricGroup(IMetricContext context, string name, HistogramConfig config = null, params string[] keys)
            => new MetricGroup<Histogram>(MetricForTagsFactory(context, name, config ?? HistogramConfig.Default), keys);

        private static Func<MetricTags, Histogram> MetricForTagsFactory(IMetricContext context, string name, HistogramConfig config)
            => tags => new Histogram(context, MetricTagsMerger.Merge(context.Tags, name, tags), config);

        #endregion
    }
}