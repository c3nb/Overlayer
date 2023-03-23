using System;
using JetBrains.Annotations;
using Vostok.Metrics.Grouping;
using Vostok.Metrics.Models;
using Vostok.Metrics.Primitives.Caching;

namespace Vostok.Metrics.Primitives.Timer
{
    [PublicAPI]
    public static class SummaryFactoryExtensions
    {
        /// <inheritdoc cref="Vostok.Metrics.Primitives.Timer.Summary"/>
        /// <param name="context">Context this metric will belong to.</param>
        /// <param name="name">Name of the metric. It will be added to event's <see cref="MetricEvent.Tags"/> with key set to <see cref="Vostok.Metrics.WellKnownTagKeys.Name"/>.</param>
        /// <param name="config">Optional metric-specific config.</param>
        [NotNull]
        public static ITimer CreateSummary([NotNull] this IMetricContext context, [NotNull] string name, [CanBeNull] SummaryConfig config = null)
            => GlobalCache.Obtain(context, name, null, () => new Summary(context, MetricTagsMerger.Merge(context.Tags, name), config ?? SummaryConfig.Default));

        #region Metric group extensions

        /// <summary>
        /// <para>
        /// Creates a group of <see cref="Vostok.Metrics.Primitives.Timer.Summary">Summarys</see>.
        /// Metrics in the group share the same context tags and <paramref name="name"/> but have different dynamic tags.
        /// </para>
        /// <para>Dynamic tags are specified by an instance of <typeparamref name="TFor"/>.</para>
        /// <para><typeparamref name="TFor"/> type must have at least one public property marked with <see cref="MetricTagAttribute"/>.</para>
        /// <inheritdoc cref="Vostok.Metrics.Primitives.Timer.Summary"/>
        /// </summary>
        /// <param name="context">Context this metric will belong to.</param>
        /// <param name="name">Name of the metric. It will be added to event's <see cref="MetricEvent.Tags"/> with key set to <see cref="Vostok.Metrics.WellKnownTagKeys.Name"/>.</param>
        /// <param name="config">Optional metric-specific config.</param>
        /// <inheritdoc cref="Vostok.Metrics.Primitives.Timer.Summary"/>
        [NotNull]
        public static IMetricGroup<TFor, ITimer> CreateSummary<TFor>([NotNull] this IMetricContext context, [NotNull] string name, [CanBeNull] SummaryConfig config = null)
            => GlobalCache.Obtain(context, name, typeof(TFor), () => new MetricGroup<TFor, ITimer>(MetricForTagsFactory(context, name, config ?? SummaryConfig.Default)));

        /// <summary>
        /// <para>
        /// Creates a group of <see cref="Vostok.Metrics.Primitives.Timer.Summary">Summarys</see>.
        /// Metrics in the group share the same context tags and <paramref name="name"/> but have different dynamic tags.
        /// </para>
        /// <para>
        /// Dynamic tags are specified by string parameters. Define the keys now and pass the values later.
        /// </para>
        /// <inheritdoc cref="Vostok.Metrics.Primitives.Timer.Summary"/>
        /// </summary>
        /// <param name="context">Context this metric will belong to.</param>
        /// <param name="name">Name of the metric. It will be added to event's <see cref="MetricEvent.Tags"/> with key set to <see cref="Vostok.Metrics.WellKnownTagKeys.Name"/>.</param>
        /// <param name="key1">Key of dynamic tag number 1.</param>
        /// <param name="config">Optional config</param>
        /// <inheritdoc cref="Vostok.Metrics.Primitives.Timer.Summary"/>
        [NotNull]
        public static IMetricGroup1<ITimer> CreateSummary([NotNull] this IMetricContext context, [NotNull] string name, [NotNull] string key1, [CanBeNull] SummaryConfig config = null)
            => GlobalCache.Obtain(context, name, key1, () => CreateMetricGroup(context, name, config, key1));

        /// <summary>
        /// <para>
        /// Creates a group of <see cref="Vostok.Metrics.Primitives.Timer.Summary">Summarys</see>.
        /// Metrics in the group share the same context tags and <paramref name="name"/> but have different dynamic tags.
        /// </para>
        /// <para>
        /// Dynamic tags are specified by string parameters. Define the keys now and pass the values later.
        /// </para>
        /// <inheritdoc cref="Vostok.Metrics.Primitives.Timer.Summary"/>
        /// </summary>
        /// <param name="context">Context this metric will belong to.</param>
        /// <param name="name">Name of the metric. It will be added to event's <see cref="MetricEvent.Tags"/> with key set to <see cref="Vostok.Metrics.WellKnownTagKeys.Name"/>.</param>
        /// <param name="key1">Key of dynamic tag number 1.</param>
        /// <param name="key2">Key of dynamic tag number 2.</param>
        /// <param name="config">Optional config</param>
        /// <inheritdoc cref="Vostok.Metrics.Primitives.Timer.Summary"/>
        [NotNull]
        public static IMetricGroup2<ITimer> CreateSummary([NotNull] this IMetricContext context, [NotNull] string name, [NotNull] string key1, [NotNull] string key2, [CanBeNull] SummaryConfig config = null)
            => GlobalCache.Obtain(context, name, (key1, key2), () => CreateMetricGroup(context, name, config, key1, key2));

        /// <summary>
        /// <para>
        /// Creates a group of <see cref="Vostok.Metrics.Primitives.Timer.Summary">Summarys</see>.
        /// Metrics in the group share the same context tags and <paramref name="name"/> but have different dynamic tags.
        /// </para>
        /// <para>
        /// Dynamic tags are specified by string parameters. Define the keys now and pass the values later.
        /// </para>
        /// <inheritdoc cref="Vostok.Metrics.Primitives.Timer.Summary"/>
        /// </summary>
        /// <param name="context">Context this metric will belong to.</param>
        /// <param name="name">Name of the metric. It will be added to event's <see cref="MetricEvent.Tags"/> with key set to <see cref="Vostok.Metrics.WellKnownTagKeys.Name"/>.</param>
        /// <param name="key1">Key of dynamic tag number 1.</param>
        /// <param name="key2">Key of dynamic tag number 2.</param>
        /// <param name="key3">Key of dynamic tag number 3.</param>
        /// <param name="config">Optional config</param>
        /// <inheritdoc cref="Vostok.Metrics.Primitives.Timer.Summary"/>
        [NotNull]
        public static IMetricGroup3<ITimer> CreateSummary([NotNull] this IMetricContext context, [NotNull] string name, [NotNull] string key1, [NotNull] string key2, [NotNull] string key3, [CanBeNull] SummaryConfig config = null)
            => GlobalCache.Obtain(context, name, (key1, key2, key3), () => CreateMetricGroup(context, name, config, key1, key2, key3));

        /// <summary>
        /// <para>
        /// Creates a group of <see cref="Vostok.Metrics.Primitives.Timer.Summary">Summarys</see>.
        /// Metrics in the group share the same context tags and <paramref name="name"/> but have different dynamic tags.
        /// </para>
        /// <para>
        /// Dynamic tags are specified by string parameters. Define the keys now and pass the values later.
        /// </para>
        /// <inheritdoc cref="Vostok.Metrics.Primitives.Timer.Summary"/>
        /// </summary>
        /// <param name="context">Context this metric will belong to.</param>
        /// <param name="name">Name of the metric. It will be added to event's <see cref="MetricEvent.Tags"/> with key set to <see cref="Vostok.Metrics.WellKnownTagKeys.Name"/>.</param>
        /// <param name="key1">Key of dynamic tag number 1.</param>
        /// <param name="key2">Key of dynamic tag number 2.</param>
        /// <param name="key3">Key of dynamic tag number 3.</param>
        /// <param name="key4">Key of dynamic tag number 4.</param>
        /// <param name="config">Optional config</param>
        /// <inheritdoc cref="Vostok.Metrics.Primitives.Timer.Summary"/>
        [NotNull]
        public static IMetricGroup4<ITimer> CreateSummary([NotNull] this IMetricContext context, [NotNull] string name, [NotNull] string key1, [NotNull] string key2, [NotNull] string key3, [NotNull] string key4, [CanBeNull] SummaryConfig config = null)
            => GlobalCache.Obtain(context, name, (key1, key2, key3, key4), () => CreateMetricGroup(context, name, config, key1, key2, key3, key4));

        /// <summary>
        /// <para>
        /// Creates a group of <see cref="Vostok.Metrics.Primitives.Timer.Summary">Summarys</see>.
        /// Metrics in the group share the same context tags and <paramref name="name"/> but have different dynamic tags.
        /// </para>
        /// <para>
        /// Dynamic tags are specified by string parameters. Define the keys now and pass the values later.
        /// </para>
        /// <inheritdoc cref="Vostok.Metrics.Primitives.Timer.Summary"/>
        /// </summary>
        /// <param name="context">Context this metric will belong to.</param>
        /// <param name="name">Name of the metric. It will be added to event's <see cref="MetricEvent.Tags"/> with key set to <see cref="Vostok.Metrics.WellKnownTagKeys.Name"/>.</param>
        /// <param name="key1">Key of dynamic tag number 1.</param>
        /// <param name="key2">Key of dynamic tag number 2.</param>
        /// <param name="key3">Key of dynamic tag number 3.</param>
        /// <param name="key4">Key of dynamic tag number 4.</param>
        /// <param name="key5">Key of dynamic tag number 5.</param>
        /// <param name="config">Optional config</param>
        /// <inheritdoc cref="Vostok.Metrics.Primitives.Timer.Summary"/>
        [NotNull]
        public static IMetricGroup5<ITimer> CreateSummary([NotNull] this IMetricContext context, [NotNull] string name, [NotNull] string key1, [NotNull] string key2, [NotNull] string key3, [NotNull] string key4, [NotNull] string key5, [CanBeNull] SummaryConfig config = null)
            => GlobalCache.Obtain(context, name, (key1, key2, key3, key4, key5), () => CreateMetricGroup(context, name, config, key1, key2, key3, key4, key5));

        /// <summary>
        /// <para>
        /// Creates a group of <see cref="Vostok.Metrics.Primitives.Timer.Summary">Summarys</see>.
        /// Metrics in the group share the same context tags and <paramref name="name"/> but have different dynamic tags.
        /// </para>
        /// <para>
        /// Dynamic tags are specified by string parameters. Define the keys now and pass the values later.
        /// </para>
        /// <inheritdoc cref="Vostok.Metrics.Primitives.Timer.Summary"/>
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
        /// <inheritdoc cref="Vostok.Metrics.Primitives.Timer.Summary"/>
        [NotNull]
        public static IMetricGroup6<ITimer> CreateSummary([NotNull] this IMetricContext context, [NotNull] string name, [NotNull] string key1, [NotNull] string key2, [NotNull] string key3, [NotNull] string key4, [NotNull] string key5, [NotNull] string key6, [CanBeNull] SummaryConfig config = null)
            => GlobalCache.Obtain(context, name, (key1, key2, key3, key4, key5, key6), () => CreateMetricGroup(context, name, config, key1, key2, key3, key4, key5, key6));

        /// <summary>
        /// <para>
        /// Creates a group of <see cref="Vostok.Metrics.Primitives.Timer.Summary">Summarys</see>.
        /// Metrics in the group share the same context tags and <paramref name="name"/> but have different dynamic tags.
        /// </para>
        /// <para>
        /// Dynamic tags are specified by string parameters. Define the keys now and pass the values later.
        /// </para>
        /// <inheritdoc cref="Vostok.Metrics.Primitives.Timer.Summary"/>
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
        /// <inheritdoc cref="Vostok.Metrics.Primitives.Timer.Summary"/>
        [NotNull]
        public static IMetricGroup7<ITimer> CreateSummary([NotNull] this IMetricContext context, [NotNull] string name, [NotNull] string key1, [NotNull] string key2, [NotNull] string key3, [NotNull] string key4, [NotNull] string key5, [NotNull] string key6, [NotNull] string key7, [CanBeNull] SummaryConfig config = null)
            => GlobalCache.Obtain(context, name, (key1, key2, key3, key4, key5, key6, key7), () => CreateMetricGroup(context, name, config, key1, key2, key3, key4, key5, key6, key7));

        /// <summary>
        /// <para>
        /// Creates a group of <see cref="Vostok.Metrics.Primitives.Timer.Summary">Summarys</see>.
        /// Metrics in the group share the same context tags and <paramref name="name"/> but have different dynamic tags.
        /// </para>
        /// <para>
        /// Dynamic tags are specified by string parameters. Define the keys now and pass the values later.
        /// </para>
        /// <inheritdoc cref="Vostok.Metrics.Primitives.Timer.Summary"/>
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
        /// <inheritdoc cref="Vostok.Metrics.Primitives.Timer.Summary"/>
        [NotNull]
        public static IMetricGroup8<ITimer> CreateSummary([NotNull] this IMetricContext context, [NotNull] string name, [NotNull] string key1, [NotNull] string key2, [NotNull] string key3, [NotNull] string key4, [NotNull] string key5, [NotNull] string key6, [NotNull] string key7, [NotNull] string key8, [CanBeNull] SummaryConfig config = null)
            => GlobalCache.Obtain(context, name, (key1, key2, key3, key4, key5, key6, key7, key8), () => CreateMetricGroup(context, name, config, key1, key2, key3, key4, key5, key6, key7, key8));

        #endregion

        #region Helper methods

        private static MetricGroup<Summary> CreateMetricGroup(IMetricContext context, string name, SummaryConfig config = null, params string[] keys)
            => new MetricGroup<Summary>(MetricForTagsFactory(context, name, config ?? SummaryConfig.Default), keys);

        private static Func<MetricTags, Summary> MetricForTagsFactory(IMetricContext context, string name, SummaryConfig config)
            => tags => new Summary(context, MetricTagsMerger.Merge(context.Tags, name, tags), config);

        #endregion
    }
}