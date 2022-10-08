using System;
using System.Diagnostics;

namespace Vostok.Sys.Metrics.PerfCounters.Utilities
{
    internal class TimeProvider : ITimeProvider
    {
        public static readonly TimeProvider Instance = new TimeProvider();

        private readonly Stopwatch stopwatch = Stopwatch.StartNew();

        public DateTime GetCurrentTime() => DateTime.UtcNow;

        public TimeSpan GetIncreasingTime() => stopwatch.Elapsed;
    }
}