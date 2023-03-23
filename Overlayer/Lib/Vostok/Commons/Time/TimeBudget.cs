using System;
using System.Diagnostics;
using JetBrains.Annotations;

namespace Vostok.Commons.Time
{
    [PublicAPI]
    internal class TimeBudget
    {
        public static readonly TimeBudget Infinite = new TimeBudget(TimeSpan.MaxValue, TimeSpan.Zero);

        public static readonly TimeBudget Expired = new TimeBudget(TimeSpan.Zero, TimeSpan.Zero);

        private static readonly TimeSpan DefaultPrecision = TimeSpan.FromMilliseconds(5);

        private readonly Stopwatch stopwatch;

        protected TimeBudget(TimeSpan total, TimeSpan precision)
        {
            if (total < TimeSpan.Zero)
                throw new ArgumentOutOfRangeException(nameof(total), $"Negative time budget value: '{total}'");

            if (precision < TimeSpan.Zero)
                throw new ArgumentOutOfRangeException(nameof(precision), $"Negative precision value: '{precision}'");

            Total = total;
            Precision = precision;

            stopwatch = new Stopwatch();
        }

        public TimeSpan Total { get; }

        public TimeSpan Precision { get; }

        public TimeSpan Elapsed => stopwatch.Elapsed;

        public TimeSpan Remaining
        {
            get
            {
                var remaining = Total - Elapsed;

                return remaining >= TimeSpan.Zero ? remaining : TimeSpan.Zero;
            }
        }

        public bool HasExpired => Total - Elapsed - Precision <= TimeSpan.Zero;

        public static TimeBudget CreateNew(TimeSpan budget)
            => new TimeBudget(budget, DefaultPrecision);

        public static TimeBudget StartNew(TimeSpan budget)
        {
            return StartNew(budget, DefaultPrecision);
        }

        public static TimeBudget StartNew(TimeSpan budget, TimeSpan precision)
        {
            var result = new TimeBudget(budget, precision);

            result.stopwatch.Start();

            return result;
        }

        public TimeSpan TryAcquire(TimeSpan needed)
        {
            var remaining = Remaining;

            return needed <= remaining ? needed : remaining;
        }

        public void Start() => stopwatch.Start();
    }
}