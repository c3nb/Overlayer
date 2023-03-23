using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Vostok.Metrics.Helpers;
using Vostok.Metrics.Primitives.Timer;

namespace Vostok.Metrics
{
    [PublicAPI]
    public static class AggregationParametersExtensions
    {
        private const string AggregatePeriodKey = "_period";
        private const string AggregateLagKey = "_lag";

        private const string QuantilesKey = "_quantiles";
        private const string QuantilesDelimiter = ";";

        private const string LowerBoundKey = WellKnownTagKeys.LowerBound;
        private const string UpperBoundKey = WellKnownTagKeys.UpperBound;

        [CanBeNull]
        public static HistogramBucket? GetHistogramBucket([CanBeNull] this IReadOnlyDictionary<string, string> aggregationParameters)
        {
            var lower = aggregationParameters.GetDouble(LowerBoundKey);
            var upper = aggregationParameters.GetDouble(UpperBoundKey);
            if (lower == null || upper == null)
                return null;
            return new HistogramBucket(lower.Value, upper.Value);
        }

        [NotNull]
        public static Dictionary<string, string> SetQuantiles([NotNull] this Dictionary<string, string> aggregationParameters, [NotNull] double[] quantiles)
        {
            aggregationParameters = aggregationParameters ?? throw new ArgumentNullException(nameof(aggregationParameters));
            quantiles = quantiles ?? throw new ArgumentNullException(nameof(quantiles));

            aggregationParameters[QuantilesKey] = string.Join(QuantilesDelimiter, quantiles.Select(DoubleSerializer.Serialize));

            return aggregationParameters;
        }

        [CanBeNull]
        public static double[] GetQuantiles([CanBeNull] this IReadOnlyDictionary<string, string> aggregationParameters)
        {
            if (aggregationParameters == null)
                return null;
            if (!aggregationParameters.TryGetValue(QuantilesKey, out var quantiles) || quantiles == null)
                return null;

            return quantiles.Split(new[] {QuantilesDelimiter}, StringSplitOptions.RemoveEmptyEntries).Select(DoubleSerializer.Deserialize).ToArray();
        }

        [NotNull]
        public static Dictionary<string, string> SetAggregationPeriod([NotNull] this Dictionary<string, string> aggregationParameters, TimeSpan period) =>
            SetTimeSpan(aggregationParameters, AggregatePeriodKey, period);

        [CanBeNull]
        public static TimeSpan? GetAggregationPeriod([CanBeNull] this IReadOnlyDictionary<string, string> aggregationParameters) =>
            GetTimeSpan(aggregationParameters, AggregatePeriodKey);

        [NotNull]
        public static Dictionary<string, string> SetAggregationLag([NotNull] this Dictionary<string, string> aggregationParameters, TimeSpan period) =>
            SetTimeSpan(aggregationParameters, AggregateLagKey, period);

        [CanBeNull]
        public static TimeSpan? GetAggregationLag([CanBeNull] this IReadOnlyDictionary<string, string> aggregationParameters) =>
            GetTimeSpan(aggregationParameters, AggregateLagKey);

        [NotNull]
        internal static Dictionary<string, string> SetHistogramBucket([NotNull] this Dictionary<string, string> aggregationParameters, HistogramBucket bucket)
        {
            aggregationParameters = aggregationParameters ?? throw new ArgumentNullException(nameof(aggregationParameters));

            aggregationParameters.SetDouble(LowerBoundKey, bucket.LowerBound);
            aggregationParameters.SetDouble(UpperBoundKey, bucket.UpperBound);

            return aggregationParameters;
        }

        [NotNull]
        private static Dictionary<string, string> SetDouble([NotNull] this Dictionary<string, string> aggregationParameters, [NotNull] string key, double value)
        {
            aggregationParameters = aggregationParameters ?? throw new ArgumentNullException(nameof(aggregationParameters));
            key = key ?? throw new ArgumentNullException(nameof(key));

            aggregationParameters[key] = DoubleSerializer.Serialize(value);

            return aggregationParameters;
        }

        [CanBeNull]
        private static double? GetDouble([CanBeNull] this IReadOnlyDictionary<string, string> aggregationParameters, [NotNull] string key)
        {
            if (aggregationParameters == null)
                return null;
            if (!aggregationParameters.TryGetValue(key, out var value) || value == null)
                return null;

            return DoubleSerializer.Deserialize(value);
        }

        [NotNull]
        private static Dictionary<string, string> SetTimeSpan([NotNull] this Dictionary<string, string> aggregationParameters, [NotNull] string key, TimeSpan timeSpan)
        {
            aggregationParameters = aggregationParameters ?? throw new ArgumentNullException(nameof(aggregationParameters));
            key = key ?? throw new ArgumentNullException(nameof(key));

            aggregationParameters[key] = TimeSpanSerializer.Serialize(timeSpan);

            return aggregationParameters;
        }

        [CanBeNull]
        private static TimeSpan? GetTimeSpan([CanBeNull] this IReadOnlyDictionary<string, string> aggregationParameters, [NotNull] string key)
        {
            if (aggregationParameters == null)
                return null;
            if (!aggregationParameters.TryGetValue(key, out var value) || value == null)
                return null;

            return TimeSpanSerializer.Deserialize(value);
        }
    }
}