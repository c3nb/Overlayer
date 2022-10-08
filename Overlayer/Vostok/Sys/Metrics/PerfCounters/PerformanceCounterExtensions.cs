using System;
using JetBrains.Annotations;

namespace Vostok.Sys.Metrics.PerfCounters
{
    [PublicAPI]
    public static class PerformanceCounterExtensions
    {
        public static bool TryObserve<T>(this IPerformanceCounter<T> counter, out T value, Action<Exception> logError = null)
        {
            try
            {
                value = counter.Observe();
                return true;
            }
            catch (Exception e)
            {
                try
                {
                    logError?.Invoke(e);
                }
                catch
                {
                    // ignored
                }

                value = default;
                return false;
            }
        }
    }
}