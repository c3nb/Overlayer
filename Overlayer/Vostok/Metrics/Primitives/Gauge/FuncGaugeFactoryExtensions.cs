using System;
using JetBrains.Annotations;
using Vostok.Metrics.Grouping;
using Vostok.Metrics.Models;

namespace Vostok.Metrics.Primitives.Gauge
{
    [PublicAPI]
    public static class FuncGaugeFactoryExtensions
    {
        /// <inheritdoc cref="IFuncGauge"/>
        /// <param name="context">Context this metric will belong to.</param>
        /// <param name="name">Name of the metric. It will be added to event's <see cref="MetricEvent.Tags"/> with key set to <see cref="Vostok.Metrics.WellKnownTagKeys.Name"/>.</param>
        /// <param name="config">Optional metric-specific config.</param>
        [NotNull]
        public static IFuncGauge CreateFuncGauge([NotNull] this IMetricContext context, [NotNull] string name, [NotNull] Func<double> valueProvider, [CanBeNull] FuncGaugeConfig config = null)
        {
            if (valueProvider == null)
                throw new ArgumentNullException(nameof(valueProvider));

            return new FuncGauge(context, MetricTagsMerger.Merge(context.Tags, name), () => valueProvider(), config ?? FuncGaugeConfig.Default);
        }

        /// <inheritdoc cref="IFuncGauge"/>
        /// <param name="context">Context this metric will belong to.</param>
        /// <param name="name">Name of the metric. It will be added to event's <see cref="MetricEvent.Tags"/> with key set to <see cref="Vostok.Metrics.WellKnownTagKeys.Name"/>.</param>
        /// <param name="config">Optional metric-specific config.</param>
        [NotNull]
        public static IFuncGauge CreateFuncGauge([NotNull] this IMetricContext context, [NotNull] string name, [NotNull] Func<double?> valueProvider, [CanBeNull] FuncGaugeConfig config = null)
        {
            if (valueProvider == null)
                throw new ArgumentNullException(nameof(valueProvider));

            return new FuncGauge(context, MetricTagsMerger.Merge(context.Tags, name), valueProvider, config ?? FuncGaugeConfig.Default);
        }

        #region Metric group extensions

        /// <summary>
        /// <para>
        /// Creates a group of <see cref="IFuncGauge">FuncGauges</see>.
        /// Metrics in the group share the same context tags and <paramref name="name"/> but have different dynamic tags.
        /// </para>
        /// <para>Dynamic tags are specified by an instance of <typeparamref name="TFor"/>.</para>
        /// <para><typeparamref name="TFor"/> type must have at least one public property marked with <see cref="MetricTagAttribute"/>.</para>
        /// <inheritdoc cref="IFuncGauge"/>
        /// </summary>
        /// <param name="context">Context this metric will belong to.</param>
        /// <param name="name">Name of the metric. It will be added to event's <see cref="MetricEvent.Tags"/> with key set to <see cref="Vostok.Metrics.WellKnownTagKeys.Name"/>.</param>
        /// <param name="config">Optional metric-specific config.</param>
        /// <inheritdoc cref="IFuncGauge"/>
        [NotNull]
        public static IMetricGroup<TFor, IFuncGauge> CreateFuncGauge<TFor>([NotNull] this IMetricContext context, [NotNull] string name, [CanBeNull] FuncGaugeConfig config = null)
            => new MetricGroup<TFor, IFuncGauge>(MetricForTagsFactory(context, name, config ?? FuncGaugeConfig.Default));

        /// <summary>
        /// <para>
        /// Creates a group of <see cref="IFuncGauge">FuncGauges</see>.
        /// Metrics in the group share the same context tags and <paramref name="name"/> but have different dynamic tags.
        /// </para>
        /// <para>
        /// Dynamic tags are specified by string parameters. Define the keys now and pass the values later.
        /// </para>
        /// <inheritdoc cref="IFuncGauge"/>
        /// </summary>
        /// <param name="context">Context this metric will belong to.</param>
        /// <param name="name">Name of the metric. It will be added to event's <see cref="MetricEvent.Tags"/> with key set to <see cref="Vostok.Metrics.WellKnownTagKeys.Name"/>.</param>
        /// <param name="key1">Key of dynamic tag number 1.</param>
        /// <param name="config">Optional config</param>
        /// <inheritdoc cref="IFuncGauge"/>
        [NotNull]
        public static IMetricGroup1<IFuncGauge> CreateFuncGauge([NotNull] this IMetricContext context, [NotNull] string name, [NotNull] string key1, [CanBeNull] FuncGaugeConfig config = null)
            => CreateMetricGroup(context, name, config, key1);

        /// <summary>
        /// <para>
        /// Creates a group of <see cref="IFuncGauge">FuncGauges</see>.
        /// Metrics in the group share the same context tags and <paramref name="name"/> but have different dynamic tags.
        /// </para>
        /// <para>
        /// Dynamic tags are specified by string parameters. Define the keys now and pass the values later.
        /// </para>
        /// <inheritdoc cref="IFuncGauge"/>
        /// </summary>
        /// <param name="context">Context this metric will belong to.</param>
        /// <param name="name">Name of the metric. It will be added to event's <see cref="MetricEvent.Tags"/> with key set to <see cref="Vostok.Metrics.WellKnownTagKeys.Name"/>.</param>
        /// <param name="key1">Key of dynamic tag number 1.</param>
        /// <param name="key2">Key of dynamic tag number 2.</param>
        /// <param name="config">Optional config</param>
        /// <inheritdoc cref="IFuncGauge"/>
        [NotNull]
        public static IMetricGroup2<IFuncGauge> CreateFuncGauge([NotNull] this IMetricContext context, [NotNull] string name, [NotNull] string key1, [NotNull] string key2, [CanBeNull] FuncGaugeConfig config = null)
            => CreateMetricGroup(context, name, config, key1, key2);

        /// <summary>
        /// <para>
        /// Creates a group of <see cref="IFuncGauge">FuncGauges</see>.
        /// Metrics in the group share the same context tags and <paramref name="name"/> but have different dynamic tags.
        /// </para>
        /// <para>
        /// Dynamic tags are specified by string parameters. Define the keys now and pass the values later.
        /// </para>
        /// <inheritdoc cref="IFuncGauge"/>
        /// </summary>
        /// <param name="context">Context this metric will belong to.</param>
        /// <param name="name">Name of the metric. It will be added to event's <see cref="MetricEvent.Tags"/> with key set to <see cref="Vostok.Metrics.WellKnownTagKeys.Name"/>.</param>
        /// <param name="key1">Key of dynamic tag number 1.</param>
        /// <param name="key2">Key of dynamic tag number 2.</param>
        /// <param name="key3">Key of dynamic tag number 3.</param>
        /// <param name="config">Optional config</param>
        /// <inheritdoc cref="IFuncGauge"/>
        [NotNull]
        public static IMetricGroup3<IFuncGauge> CreateFuncGauge([NotNull] this IMetricContext context, [NotNull] string name, [NotNull] string key1, [NotNull] string key2, [NotNull] string key3, [CanBeNull] FuncGaugeConfig config = null)
            => CreateMetricGroup(context, name, config, key1, key2, key3);

        /// <summary>
        /// <para>
        /// Creates a group of <see cref="IFuncGauge">FuncGauges</see>.
        /// Metrics in the group share the same context tags and <paramref name="name"/> but have different dynamic tags.
        /// </para>
        /// <para>
        /// Dynamic tags are specified by string parameters. Define the keys now and pass the values later.
        /// </para>
        /// <inheritdoc cref="IFuncGauge"/>
        /// </summary>
        /// <param name="context">Context this metric will belong to.</param>
        /// <param name="name">Name of the metric. It will be added to event's <see cref="MetricEvent.Tags"/> with key set to <see cref="Vostok.Metrics.WellKnownTagKeys.Name"/>.</param>
        /// <param name="key1">Key of dynamic tag number 1.</param>
        /// <param name="key2">Key of dynamic tag number 2.</param>
        /// <param name="key3">Key of dynamic tag number 3.</param>
        /// <param name="key4">Key of dynamic tag number 4.</param>
        /// <param name="config">Optional config</param>
        /// <inheritdoc cref="IFuncGauge"/>
        [NotNull]
        public static IMetricGroup4<IFuncGauge> CreateFuncGauge([NotNull] this IMetricContext context, [NotNull] string name, [NotNull] string key1, [NotNull] string key2, [NotNull] string key3, [NotNull] string key4, [CanBeNull] FuncGaugeConfig config = null)
            => CreateMetricGroup(context, name, config, key1, key2, key3, key4);

        /// <summary>
        /// <para>
        /// Creates a group of <see cref="IFuncGauge">FuncGauges</see>.
        /// Metrics in the group share the same context tags and <paramref name="name"/> but have different dynamic tags.
        /// </para>
        /// <para>
        /// Dynamic tags are specified by string parameters. Define the keys now and pass the values later.
        /// </para>
        /// <inheritdoc cref="IFuncGauge"/>
        /// </summary>
        /// <param name="context">Context this metric will belong to.</param>
        /// <param name="name">Name of the metric. It will be added to event's <see cref="MetricEvent.Tags"/> with key set to <see cref="Vostok.Metrics.WellKnownTagKeys.Name"/>.</param>
        /// <param name="key1">Key of dynamic tag number 1.</param>
        /// <param name="key2">Key of dynamic tag number 2.</param>
        /// <param name="key3">Key of dynamic tag number 3.</param>
        /// <param name="key4">Key of dynamic tag number 4.</param>
        /// <param name="key5">Key of dynamic tag number 5.</param>
        /// <param name="config">Optional config</param>
        /// <inheritdoc cref="IFuncGauge"/>
        [NotNull]
        public static IMetricGroup5<IFuncGauge> CreateFuncGauge([NotNull] this IMetricContext context, [NotNull] string name, [NotNull] string key1, [NotNull] string key2, [NotNull] string key3, [NotNull] string key4, [NotNull] string key5, [CanBeNull] FuncGaugeConfig config = null)
            => CreateMetricGroup(context, name, config, key1, key2, key3, key4, key5);

        /// <summary>
        /// <para>
        /// Creates a group of <see cref="IFuncGauge">FuncGauges</see>.
        /// Metrics in the group share the same context tags and <paramref name="name"/> but have different dynamic tags.
        /// </para>
        /// <para>
        /// Dynamic tags are specified by string parameters. Define the keys now and pass the values later.
        /// </para>
        /// <inheritdoc cref="IFuncGauge"/>
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
        /// <inheritdoc cref="IFuncGauge"/>
        [NotNull]
        public static IMetricGroup6<IFuncGauge> CreateFuncGauge([NotNull] this IMetricContext context, [NotNull] string name, [NotNull] string key1, [NotNull] string key2, [NotNull] string key3, [NotNull] string key4, [NotNull] string key5, [NotNull] string key6, [CanBeNull] FuncGaugeConfig config = null)
            => CreateMetricGroup(context, name, config, key1, key2, key3, key4, key5, key6);

        /// <summary>
        /// <para>
        /// Creates a group of <see cref="IFuncGauge">FuncGauges</see>.
        /// Metrics in the group share the same context tags and <paramref name="name"/> but have different dynamic tags.
        /// </para>
        /// <para>
        /// Dynamic tags are specified by string parameters. Define the keys now and pass the values later.
        /// </para>
        /// <inheritdoc cref="IFuncGauge"/>
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
        /// <inheritdoc cref="IFuncGauge"/>
        [NotNull]
        public static IMetricGroup7<IFuncGauge> CreateFuncGauge([NotNull] this IMetricContext context, [NotNull] string name, [NotNull] string key1, [NotNull] string key2, [NotNull] string key3, [NotNull] string key4, [NotNull] string key5, [NotNull] string key6, [NotNull] string key7, [CanBeNull] FuncGaugeConfig config = null)
            => CreateMetricGroup(context, name, config, key1, key2, key3, key4, key5, key6, key7);

        /// <summary>
        /// <para>
        /// Creates a group of <see cref="IFuncGauge">FuncGauges</see>.
        /// Metrics in the group share the same context tags and <paramref name="name"/> but have different dynamic tags.
        /// </para>
        /// <para>
        /// Dynamic tags are specified by string parameters. Define the keys now and pass the values later.
        /// </para>
        /// <inheritdoc cref="IFuncGauge"/>
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
        /// <inheritdoc cref="IFuncGauge"/>
        [NotNull]
        public static IMetricGroup8<IFuncGauge> CreateFuncGauge([NotNull] this IMetricContext context, [NotNull] string name, [NotNull] string key1, [NotNull] string key2, [NotNull] string key3, [NotNull] string key4, [NotNull] string key5, [NotNull] string key6, [NotNull] string key7, [NotNull] string key8, [CanBeNull] FuncGaugeConfig config = null)
            => CreateMetricGroup(context, name, config, key1, key2, key3, key4, key5, key6, key7, key8);

        #endregion

        #region Helper methods

        private static MetricGroup<FuncGauge> CreateMetricGroup(IMetricContext context, string name, FuncGaugeConfig config = null, params string[] keys)
            => new MetricGroup<FuncGauge>(MetricForTagsFactory(context, name, config ?? FuncGaugeConfig.Default), keys);

        private static Func<MetricTags, FuncGauge> MetricForTagsFactory(IMetricContext context, string name, FuncGaugeConfig config)
            => tags => new FuncGauge(context, MetricTagsMerger.Merge(context.Tags, name, tags), config);

        #endregion
    }
}