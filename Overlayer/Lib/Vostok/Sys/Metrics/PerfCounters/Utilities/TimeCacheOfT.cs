using System;

namespace Vostok.Sys.Metrics.PerfCounters.Utilities
{
    internal class TimeCache<T>
        where T : class
    {
        private readonly Func<T> measure;
        private readonly Func<TimeSpan> getMinPauseBetweenObservations;
        private readonly ITimeProvider timeProvider;
        private readonly object sync = new object();

        private TimeSpan lastMeasureTime;
        private volatile T cachedValue;

        public TimeCache(
            Func<T> measure,
            Func<TimeSpan> getMinPauseBetweenObservations,
            ITimeProvider timeProvider = null)
        {
            this.measure = measure;
            this.getMinPauseBetweenObservations = getMinPauseBetweenObservations;
            this.timeProvider = timeProvider ?? TimeProvider.Instance;
            lastMeasureTime = TimeSpan.MinValue;
        }

        public T GetValue()
        {
            if (!NeedRefresh())
                return cachedValue;
            lock (sync)
            {
                if (!NeedRefresh())
                    return cachedValue;

                var result = measure();
                cachedValue = result;
                lastMeasureTime = timeProvider.GetIncreasingTime();

                return result;
            }
        }

        public void Evict()
        {
            lock (sync)
                lastMeasureTime = TimeSpan.MinValue;
        }

        private bool NeedRefresh()
        {
            return lastMeasureTime + getMinPauseBetweenObservations() <= timeProvider.GetIncreasingTime();
        }
    }
}