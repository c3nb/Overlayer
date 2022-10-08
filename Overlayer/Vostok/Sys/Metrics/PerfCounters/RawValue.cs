namespace Vostok.Sys.Metrics.PerfCounters
{
    /// <summary>
    /// A raw value of performance counter.
    /// </summary>
    internal readonly struct RawValue
    {
        /// <summary>
        /// A current raw counter value.
        /// </summary>
        public readonly long FirstValue;
        internal RawValue(long first) => FirstValue = first;
    }
}