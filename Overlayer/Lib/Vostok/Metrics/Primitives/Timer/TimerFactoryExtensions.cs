using System;
using JetBrains.Annotations;
using Vostok.Metrics.Grouping;
using Vostok.Metrics.Models;

namespace Vostok.Metrics.Primitives.Timer
{
    [PublicAPI]
    public static class TimerFactoryExtensions
    {
        /// <inheritdoc cref="Vostok.Metrics.Primitives.Timer.Timer"/>
        /// <param name="context">Context this metric will belong to.</param>
        /// <param name="name">Name of the metric. It will be added to event's <see cref="MetricEvent.Tags"/> with key set to <see cref="Vostok.Metrics.WellKnownTagKeys.Name"/>.</param>
        /// <param name="config">Optional metric-specific config.</param>
        [NotNull]
        public static ITimer CreateTimer([NotNull] this IMetricContext context, [NotNull] string name, [CanBeNull] TimerConfig config = null)
            => new Timer(context, MetricTagsMerger.Merge(context.Tags, name), config ?? TimerConfig.Default);

        #region Metric group extensions

        /// <summary>
        /// <para>
        /// Creates a group of <see cref="Vostok.Metrics.Primitives.Timer.Timer">Timers</see>.
        /// Metrics in the group share the same context tags and <paramref name="name"/> but have different dynamic tags.
        /// </para>
        /// <para>Dynamic tags are specified by an instance of <typeparamref name="TFor"/>.</para>
        /// <para><typeparamref name="TFor"/> type must have at least one public property marked with <see cref="MetricTagAttribute"/>.</para>
        /// <inheritdoc cref="Vostok.Metrics.Primitives.Timer.Timer"/>
        /// </summary>
        /// <param name="context">Context this metric will belong to.</param>
        /// <param name="name">Name of the metric. It will be added to event's <see cref="MetricEvent.Tags"/> with key set to <see cref="Vostok.Metrics.WellKnownTagKeys.Name"/>.</param>
        /// <param name="config">Optional metric-specific config.</param>
        /// <inheritdoc cref="Vostok.Metrics.Primitives.Timer.Timer"/>
        [NotNull]
        public static IMetricGroup<TFor, ITimer> CreateTimer<TFor>([NotNull] this IMetricContext context, [NotNull] string name, [CanBeNull] TimerConfig config = null)
            => new MetricGroup<TFor, ITimer>(MetricForTagsFactory(context, name, config ?? TimerConfig.Default));

        /// <summary>
        /// <para>
        /// Creates a group of <see cref="Vostok.Metrics.Primitives.Timer.Timer">Timers</see>.
        /// Metrics in the group share the same context tags and <paramref name="name"/> but have different dynamic tags.
        /// </para>
        /// <para>
        /// Dynamic tags are specified by string parameters. Define the keys now and pass the values later.
        /// </para>
        /// <inheritdoc cref="Vostok.Metrics.Primitives.Timer.Timer"/>
        /// </summary>
        /// <param name="context">Context this metric will belong to.</param>
        /// <param name="name">Name of the metric. It will be added to event's <see cref="MetricEvent.Tags"/> with key set to <see cref="Vostok.Metrics.WellKnownTagKeys.Name"/>.</param>
        /// <param name="key1">Key of dynamic tag number 1.</param>
        /// <param name="config">Optional config</param>
        /// <inheritdoc cref="Vostok.Metrics.Primitives.Timer.Timer"/>
        [NotNull]
        public static IMetricGroup1<ITimer> CreateTimer([NotNull] this IMetricContext context, [NotNull] string name, [NotNull] string key1, [CanBeNull] TimerConfig config = null)
            => CreateMetricGroup(context, name, config, key1);

        /// <summary>
        /// <para>
        /// Creates a group of <see cref="Vostok.Metrics.Primitives.Timer.Timer">Timers</see>.
        /// Metrics in the group share the same context tags and <paramref name="name"/> but have different dynamic tags.
        /// </para>
        /// <para>
        /// Dynamic tags are specified by string parameters. Define the keys now and pass the values later.
        /// </para>
        /// <inheritdoc cref="Vostok.Metrics.Primitives.Timer.Timer"/>
        /// </summary>
        /// <param name="context">Context this metric will belong to.</param>
        /// <param name="name">Name of the metric. It will be added to event's <see cref="MetricEvent.Tags"/> with key set to <see cref="Vostok.Metrics.WellKnownTagKeys.Name"/>.</param>
        /// <param name="key1">Key of dynamic tag number 1.</param>
        /// <param name="key2">Key of dynamic tag number 2.</param>
        /// <param name="config">Optional config</param>
        /// <inheritdoc cref="Vostok.Metrics.Primitives.Timer.Timer"/>
        [NotNull]
        public static IMetricGroup2<ITimer> CreateTimer([NotNull] this IMetricContext context, [NotNull] string name, [NotNull] string key1, [NotNull] string key2, [CanBeNull] TimerConfig config = null)
            => CreateMetricGroup(context, name, config, key1, key2);

        /// <summary>
        /// <para>
        /// Creates a group of <see cref="Vostok.Metrics.Primitives.Timer.Timer">Timers</see>.
        /// Metrics in the group share the same context tags and <paramref name="name"/> but have different dynamic tags.
        /// </para>
        /// <para>
        /// Dynamic tags are specified by string parameters. Define the keys now and pass the values later.
        /// </para>
        /// <inheritdoc cref="Vostok.Metrics.Primitives.Timer.Timer"/>
        /// </summary>
        /// <param name="context">Context this metric will belong to.</param>
        /// <param name="name">Name of the metric. It will be added to event's <see cref="MetricEvent.Tags"/> with key set to <see cref="Vostok.Metrics.WellKnownTagKeys.Name"/>.</param>
        /// <param name="key1">Key of dynamic tag number 1.</param>
        /// <param name="key2">Key of dynamic tag number 2.</param>
        /// <param name="key3">Key of dynamic tag number 3.</param>
        /// <param name="config">Optional config</param>
        /// <inheritdoc cref="Vostok.Metrics.Primitives.Timer.Timer"/>
        [NotNull]
        public static IMetricGroup3<ITimer> CreateTimer([NotNull] this IMetricContext context, [NotNull] string name, [NotNull] string key1, [NotNull] string key2, [NotNull] string key3, [CanBeNull] TimerConfig config = null)
            => CreateMetricGroup(context, name, config, key1, key2, key3);

        /// <summary>
        /// <para>
        /// Creates a group of <see cref="Vostok.Metrics.Primitives.Timer.Timer">Timers</see>.
        /// Metrics in the group share the same context tags and <paramref name="name"/> but have different dynamic tags.
        /// </para>
        /// <para>
        /// Dynamic tags are specified by string parameters. Define the keys now and pass the values later.
        /// </para>
        /// <inheritdoc cref="Vostok.Metrics.Primitives.Timer.Timer"/>
        /// </summary>
        /// <param name="context">Context this metric will belong to.</param>
        /// <param name="name">Name of the metric. It will be added to event's <see cref="MetricEvent.Tags"/> with key set to <see cref="Vostok.Metrics.WellKnownTagKeys.Name"/>.</param>
        /// <param name="key1">Key of dynamic tag number 1.</param>
        /// <param name="key2">Key of dynamic tag number 2.</param>
        /// <param name="key3">Key of dynamic tag number 3.</param>
        /// <param name="key4">Key of dynamic tag number 4.</param>
        /// <param name="config">Optional config</param>
        /// <inheritdoc cref="Vostok.Metrics.Primitives.Timer.Timer"/>
        [NotNull]
        public static IMetricGroup4<ITimer> CreateTimer([NotNull] this IMetricContext context, [NotNull] string name, [NotNull] string key1, [NotNull] string key2, [NotNull] string key3, [NotNull] string key4, [CanBeNull] TimerConfig config = null)
            => CreateMetricGroup(context, name, config, key1, key2, key3, key4);

        /// <summary>
        /// <para>
        /// Creates a group of <see cref="Vostok.Metrics.Primitives.Timer.Timer">Timers</see>.
        /// Metrics in the group share the same context tags and <paramref name="name"/> but have different dynamic tags.
        /// </para>
        /// <para>
        /// Dynamic tags are specified by string parameters. Define the keys now and pass the values later.
        /// </para>
        /// <inheritdoc cref="Vostok.Metrics.Primitives.Timer.Timer"/>
        /// </summary>
        /// <param name="context">Context this metric will belong to.</param>
        /// <param name="name">Name of the metric. It will be added to event's <see cref="MetricEvent.Tags"/> with key set to <see cref="Vostok.Metrics.WellKnownTagKeys.Name"/>.</param>
        /// <param name="key1">Key of dynamic tag number 1.</param>
        /// <param name="key2">Key of dynamic tag number 2.</param>
        /// <param name="key3">Key of dynamic tag number 3.</param>
        /// <param name="key4">Key of dynamic tag number 4.</param>
        /// <param name="key5">Key of dynamic tag number 5.</param>
        /// <param name="config">Optional config</param>
        /// <inheritdoc cref="Vostok.Metrics.Primitives.Timer.Timer"/>
        [NotNull]
        public static IMetricGroup5<ITimer> CreateTimer([NotNull] this IMetricContext context, [NotNull] string name, [NotNull] string key1, [NotNull] string key2, [NotNull] string key3, [NotNull] string key4, [NotNull] string key5, [CanBeNull] TimerConfig config = null)
            => CreateMetricGroup(context, name, config, key1, key2, key3, key4, key5);

        /// <summary>
        /// <para>
        /// Creates a group of <see cref="Vostok.Metrics.Primitives.Timer.Timer">Timers</see>.
        /// Metrics in the group share the same context tags and <paramref name="name"/> but have different dynamic tags.
        /// </para>
        /// <para>
        /// Dynamic tags are specified by string parameters. Define the keys now and pass the values later.
        /// </para>
        /// <inheritdoc cref="Vostok.Metrics.Primitives.Timer.Timer"/>
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
        /// <inheritdoc cref="Vostok.Metrics.Primitives.Timer.Timer"/>
        [NotNull]
        public static IMetricGroup6<ITimer> CreateTimer([NotNull] this IMetricContext context, [NotNull] string name, [NotNull] string key1, [NotNull] string key2, [NotNull] string key3, [NotNull] string key4, [NotNull] string key5, [NotNull] string key6, [CanBeNull] TimerConfig config = null)
            => CreateMetricGroup(context, name, config, key1, key2, key3, key4, key5, key6);

        /// <summary>
        /// <para>
        /// Creates a group of <see cref="Vostok.Metrics.Primitives.Timer.Timer">Timers</see>.
        /// Metrics in the group share the same context tags and <paramref name="name"/> but have different dynamic tags.
        /// </para>
        /// <para>
        /// Dynamic tags are specified by string parameters. Define the keys now and pass the values later.
        /// </para>
        /// <inheritdoc cref="Vostok.Metrics.Primitives.Timer.Timer"/>
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
        /// <inheritdoc cref="Vostok.Metrics.Primitives.Timer.Timer"/>
        [NotNull]
        public static IMetricGroup7<ITimer> CreateTimer([NotNull] this IMetricContext context, [NotNull] string name, [NotNull] string key1, [NotNull] string key2, [NotNull] string key3, [NotNull] string key4, [NotNull] string key5, [NotNull] string key6, [NotNull] string key7, [CanBeNull] TimerConfig config = null)
            => CreateMetricGroup(context, name, config, key1, key2, key3, key4, key5, key6, key7);

        /// <summary>
        /// <para>
        /// Creates a group of <see cref="Vostok.Metrics.Primitives.Timer.Timer">Timers</see>.
        /// Metrics in the group share the same context tags and <paramref name="name"/> but have different dynamic tags.
        /// </para>
        /// <para>
        /// Dynamic tags are specified by string parameters. Define the keys now and pass the values later.
        /// </para>
        /// <inheritdoc cref="Vostok.Metrics.Primitives.Timer.Timer"/>
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
        /// <inheritdoc cref="Vostok.Metrics.Primitives.Timer.Timer"/>
        [NotNull]
        public static IMetricGroup8<ITimer> CreateTimer([NotNull] this IMetricContext context, [NotNull] string name, [NotNull] string key1, [NotNull] string key2, [NotNull] string key3, [NotNull] string key4, [NotNull] string key5, [NotNull] string key6, [NotNull] string key7, [NotNull] string key8, [CanBeNull] TimerConfig config = null)
            => CreateMetricGroup(context, name, config, key1, key2, key3, key4, key5, key6, key7, key8);

        #endregion

        #region Helper methods

        private static MetricGroup<Timer> CreateMetricGroup(IMetricContext context, string name, TimerConfig config = null, params string[] keys)
            => new MetricGroup<Timer>(MetricForTagsFactory(context, name, config ?? TimerConfig.Default), keys);

        private static Func<MetricTags, Timer> MetricForTagsFactory(IMetricContext context, string name, TimerConfig config)
            => tags => new Timer(context, MetricTagsMerger.Merge(context.Tags, name, tags), config);

        #endregion
    }
}