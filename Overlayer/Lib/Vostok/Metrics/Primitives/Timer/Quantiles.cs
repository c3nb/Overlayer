using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Vostok.Metrics.Models;

namespace Vostok.Metrics.Primitives.Timer
{
    /// <summary>
    /// Quantiles helper class.
    /// </summary>
    [PublicAPI]
    public static class Quantiles
    {
        public static double[] DefaultQuantiles => new[] {0.5, 0.75, 0.95, 0.99, 0.999};

        /// <summary>
        /// Calculates quantile by array of sorted <paramref name="values"/>.
        /// </summary>
        /// <param name="size">The number of values to be used.</param>
        public static double GetQuantile(double quantile, IList<double> values, int size)
        {
            if (size == 0)
                return 0;

            var k = (int)(quantile * (size - 1));

            var index = k + 1 <= quantile * size ? k + 1 : k;

            if (index < 0)
                index = 0;
            if (index >= size)
                index = size - 1;

            return values[index];
        }

        /// <summary>
        /// Appends quantile tag with <see cref="WellKnownTagKeys.Aggregate"/> key and 'pXX' value to <paramref name="baseTags"/>.
        /// </summary>
        public static MetricTags[] QuantileTags(double[] quantiles, MetricTags baseTags)
        {
            var quantileTags = new MetricTags[quantiles.Length];

            for (var i = 0; i < quantiles.Length; i++)
                quantileTags[i] = baseTags.Append(QuantileTag(quantiles[i]));

            return quantileTags;
        }

        public static MetricTag QuantileTag(double quantile)
        {
            const double epsilon = 1e-9;

            if (quantile < 0d || quantile > 1d)
                throw new ArgumentOutOfRangeException(nameof(quantile), $"Provided quantile has incorrect value '{quantile}'.");

            quantile *= 100d;

            for (var d = 0; d < 6; d++)
            {
                if (Math.Abs(Math.Round(quantile, MidpointRounding.AwayFromZero) - quantile) < epsilon)
                    break;
                quantile *= 10d;
            }

            return new MetricTag(WellKnownTagKeys.Aggregate, "p" + (int)Math.Round(quantile, MidpointRounding.AwayFromZero));
        }
    }
}