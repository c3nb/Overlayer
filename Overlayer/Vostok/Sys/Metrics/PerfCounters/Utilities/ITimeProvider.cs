using System;

namespace Vostok.Sys.Metrics.PerfCounters.Utilities
{
    internal interface ITimeProvider
    {
        DateTime GetCurrentTime();
        TimeSpan GetIncreasingTime();
    }
}