using System;
using JetBrains.Annotations;

namespace Vostok.Metrics.Primitives.Timer
{
    [PublicAPI]
    public struct HistogramBucket
    {
        public readonly double LowerBound;
        public readonly double UpperBound;

        public HistogramBucket(double lowerBound, double upperBound)
        {
            if (lowerBound >= upperBound)
                throw new ArgumentException($"Incorrect bucket bounds: lower bound {lowerBound} >= upper bound {upperBound}.");

            LowerBound = lowerBound;
            UpperBound = upperBound;
        }

        #region Equality

        public bool Equals(HistogramBucket other) =>
            LowerBound.Equals(other.LowerBound) && UpperBound.Equals(other.UpperBound);

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            return obj is HistogramBucket other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (LowerBound.GetHashCode() * 397) ^ UpperBound.GetHashCode();
            }
        }
    }

    #endregion
}