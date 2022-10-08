using System;

namespace Vostok.Sys.Metrics.PerfCounters
{
    /// <summary>
    /// <para>Represents a Windows Performance counter.</para>
    /// <para>Implementations of this interface uses PDH library internally and holds a PdhQuery handle.</para>
    /// <para>You should reuse instances of this interface and Dispose it.</para>
    /// </summary>
    /// <typeparam name="T">A type of complex counter value.</typeparam>
    public interface IPerformanceCounter<T> : IDisposable
    {
        /// <returns>Observed performance counters value.</returns>
        T Observe();
    }
}