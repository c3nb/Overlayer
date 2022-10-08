using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace Vostok.Metrics.Primitives.Timer
{
    [PublicAPI]
    public class HistogramBuckets
    {
        private readonly IReadOnlyList<double> upperBounds;

        /// <param name="upperBounds">
        /// <para>An array of inclusive upper bounds of histogram's buckets.</para>
        /// <para>Must contain a monotonically increasing sequence of values.</para>
        /// <para>Must not be empty.</para>
        /// </param>
        public HistogramBuckets([NotNull] params double[] upperBounds)
            : this(upperBounds as IReadOnlyList<double>)
        {
        }

        /// <param name="upperBounds">
        /// <para>A list of inclusive upper bounds of histogram's buckets.</para>
        /// <para>Must contain a monotonically increasing sequence of values.</para>
        /// <para>Must not be empty.</para>
        /// </param>
        public HistogramBuckets([NotNull] IReadOnlyList<double> upperBounds)
        {
            if (upperBounds == null)
                throw new ArgumentNullException(nameof(upperBounds));

            if (upperBounds.Count == 0)
                throw new ArgumentException("Provided upper bounds list was empty.");

            for (var i = 1; i < upperBounds.Count; i++)
            {
                var currentBound = upperBounds[i];
                var previousBound = upperBounds[i - 1];
                if (previousBound >= currentBound)
                    throw new ArgumentException("Upper bounds sequence must be increasing.");
            }

            this.upperBounds = upperBounds;

            Count = upperBounds.Count + 1;
        }

        [NotNull]
        public static HistogramBuckets CreateLinear(double start, double width, int count)
        {
            if (count <= 0)
                throw new ArgumentOutOfRangeException(nameof(count), count, "Buckets count must be positive.");

            var upperBounds = new double[count];

            for (var i = 0; i < count; i++)
            {
                upperBounds[i] = start;

                start += width;
            }

            return new HistogramBuckets(upperBounds);
        }

        [NotNull]
        public static HistogramBuckets CreateExponential(double start, double factor, int count)
        {
            if (count <= 0)
                throw new ArgumentOutOfRangeException(nameof(count), count, "Buckets count must be positive.");

            if (start <= 0)
                throw new ArgumentOutOfRangeException(nameof(start), start, "Starting value must be positive.");

            if (factor <= 1)
                throw new ArgumentOutOfRangeException(nameof(factor), factor, "Exponential factor must be > 1.");

            var upperBounds = new double[count];

            for (var i = 0; i < count; i++)
            {
                upperBounds[i] = start;

                start *= factor;
            }

            return new HistogramBuckets(upperBounds);
        }

        public int Count { get; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int FindBucketIndex(double value)
        {
            var l = -1;
            var r = upperBounds.Count;

            // Note(kungurtsev): invariant value <= upperBounds[r]

            while (l + 1 < r)
            {
                var m = (l + r) / 2;
                if (value <= upperBounds[m])
                    r = m;
                else
                    l = m;
            }

            return r;
        }

        public HistogramBucket this[int index]
        {
            get
            {
                if (index < 0 || index >= Count)
                    throw new IndexOutOfRangeException();

                var lowerBound = index == 0 ? double.NegativeInfinity : upperBounds[index - 1];
                var upperBound = index == Count - 1 ? double.PositiveInfinity : upperBounds[index];

                return new HistogramBucket(lowerBound, upperBound);
            }
        }
    }
}