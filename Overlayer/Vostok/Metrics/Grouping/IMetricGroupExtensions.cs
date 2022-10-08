using System;
using System.Globalization;
using JetBrains.Annotations;

namespace Vostok.Metrics.Grouping
{
    [PublicAPI]
    public static class IMetricGroupExtensions
    {
        /// <inheritdoc cref="IMetricGroup1{TMetric}.For"/>
        [NotNull]
        public static TMetric For<TMetric, TValue1>([NotNull] this IMetricGroup1<TMetric> metric, [NotNull] TValue1 value1)
            => metric.For(ToString(value1));

        /// <inheritdoc cref="IMetricGroup2{TMetric}.For"/>
        [NotNull]
        public static TMetric For<TMetric, TValue1, TValue2>([NotNull] this IMetricGroup2<TMetric> metric, [NotNull] TValue1 value1, [NotNull] TValue2 value2)
            => metric.For(ToString(value1), ToString(value2));

        /// <inheritdoc cref="IMetricGroup3{TMetric}.For"/>
        [NotNull]
        public static TMetric For<TMetric, TValue1, TValue2, TValue3>([NotNull] this IMetricGroup3<TMetric> metric, [NotNull] TValue1 value1, [NotNull] TValue2 value2, [NotNull] TValue3 value3)
            => metric.For(ToString(value1), ToString(value2), ToString(value3));

        /// <inheritdoc cref="IMetricGroup4{TMetric}.For"/>
        [NotNull]
        public static TMetric For<TMetric, TValue1, TValue2, TValue3, TValue4>([NotNull] this IMetricGroup4<TMetric> metric, [NotNull] TValue1 value1, [NotNull] TValue2 value2, [NotNull] TValue3 value3, [NotNull] TValue4 value4)
            => metric.For(ToString(value1), ToString(value2), ToString(value3), ToString(value4));

        /// <inheritdoc cref="IMetricGroup5{TMetric}.For"/>
        [NotNull]
        public static TMetric For<TMetric, TValue1, TValue2, TValue3, TValue4, TValue5>([NotNull] this IMetricGroup5<TMetric> metric, [NotNull] TValue1 value1, [NotNull] TValue2 value2, [NotNull] TValue3 value3, [NotNull] TValue4 value4, [NotNull] TValue5 value5)
            => metric.For(ToString(value1), ToString(value2), ToString(value3), ToString(value4), ToString(value5));

        /// <inheritdoc cref="IMetricGroup6{TMetric}.For"/>
        [NotNull]
        public static TMetric For<TMetric, TValue1, TValue2, TValue3, TValue4, TValue5, TValue6>([NotNull] this IMetricGroup6<TMetric> metric, [NotNull] TValue1 value1, [NotNull] TValue2 value2, [NotNull] TValue3 value3, [NotNull] TValue4 value4, [NotNull] TValue5 value5, [NotNull] TValue6 value6)
            => metric.For(ToString(value1), ToString(value2), ToString(value3), ToString(value4), ToString(value5), ToString(value6));

        /// <inheritdoc cref="IMetricGroup7{TMetric}.For"/>
        [NotNull]
        public static TMetric For<TMetric, TValue1, TValue2, TValue3, TValue4, TValue5, TValue6, TValue7>([NotNull] this IMetricGroup7<TMetric> metric, [NotNull] TValue1 value1, [NotNull] TValue2 value2, [NotNull] TValue3 value3, [NotNull] TValue4 value4, [NotNull] TValue5 value5, [NotNull] TValue6 value6, [NotNull] TValue7 value7)
            => metric.For(ToString(value1), ToString(value2), ToString(value3), ToString(value4), ToString(value5), ToString(value6), ToString(value7));

        /// <inheritdoc cref="IMetricGroup8{TMetric}.For"/>
        [NotNull]
        public static TMetric For<TMetric, TValue1, TValue2, TValue3, TValue4, TValue5, TValue6, TValue7, TValue8>([NotNull] this IMetricGroup8<TMetric> metric, [NotNull] TValue1 value1, [NotNull] TValue2 value2, [NotNull] TValue3 value3, [NotNull] TValue4 value4, [NotNull] TValue5 value5, [NotNull] TValue6 value6, [NotNull] TValue7 value7, [NotNull] TValue8 value8)
            => metric.For(ToString(value1), ToString(value2), ToString(value3), ToString(value4), ToString(value5), ToString(value6), ToString(value7), ToString(value8));

        private static string ToString<T>(T value)
        {
            var result = (value as IFormattable)?.ToString(null, CultureInfo.InvariantCulture) ?? value?.ToString();

            if (string.IsNullOrEmpty(result))
                return "none";

            return result;
        }
    }
}