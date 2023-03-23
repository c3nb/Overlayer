using System;
using System.Threading;

namespace Vostok.Sys.Metrics.PerfCounters.Utilities
{
    internal class TimeCache
    {
        private readonly Func<double> measure;
        private readonly Func<TimeSpan> getMinPauseBetweenObservations;
        private readonly ITimeProvider timeProvider;

        private int sync;
        private DateTime lastMeasureTime;
        private double cachedValue;

        public TimeCache(
            Func<double> measure,
            Func<TimeSpan> getMinPauseBetweenObservations,
            ITimeProvider timeProvider = null)
        {
            this.measure = measure;
            this.getMinPauseBetweenObservations = getMinPauseBetweenObservations;
            this.timeProvider = timeProvider ?? TimeProvider.Instance;
            lastMeasureTime = DateTime.MinValue;
            sync = 0;
        }

        public double GetValue()
        {
            try
            {
                if (Interlocked.Increment(ref sync) == 1)
                {
                    var now = timeProvider.GetCurrentTime();
                    if (lastMeasureTime + getMinPauseBetweenObservations() <= now)
                    {
                        var result = measure();
                        Interlocked.Exchange(ref cachedValue, result);
                        lastMeasureTime = now;
                    }
                }

                return cachedValue;
            }
            finally
            {
                Interlocked.Decrement(ref sync);
            }
        }
    }
}