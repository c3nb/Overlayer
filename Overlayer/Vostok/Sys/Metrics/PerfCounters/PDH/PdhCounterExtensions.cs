namespace Vostok.Sys.Metrics.PerfCounters.PDH
{
    internal static class PdhCounterExtensions
    {
        private const PdhFmt DefaultFmt = PdhFmt.PDH_FMT_DOUBLE | PdhFmt.PDH_FMT_NOCAP100;

        public static PdhStatus GetFormattedValue(
            this PdhCounter counter,
            out PDH_FMT_COUNTERVALUE value)
            => PdhExports.PdhGetFormattedCounterValue(
                counter,
                DefaultFmt,
                out _,
                out value);

        public static PdhStatus GetRawValue(
            this PdhCounter counter,
            out PDH_RAW_COUNTER value)
            => PdhExports.PdhGetRawCounterValue(
                counter,
                out _,
                out value);

        public static unsafe PdhStatus GetRawCounterArray(
            this PdhCounter counter,
            ref int bufferSize,
            out int itemCount,
            PDH_RAW_COUNTER_ITEM* buffer)
            => PdhExports.PdhGetRawCounterArray(counter, ref bufferSize, out itemCount, buffer);

        public static unsafe PdhStatus GetFormattedCounterArray(
            this PdhCounter counter,
            ref int bufferSize,
            out int itemCount,
            PDH_FMT_COUNTERVALUE_ITEM* buffer)
            => PdhExports.PdhGetFormattedCounterArray(counter, DefaultFmt, ref bufferSize, out itemCount, buffer);

        public static unsafe int EstimateRawCounterArraySize(this PdhCounter counter)
        {
            var size = 0;

            PdhExports
                .PdhGetRawCounterArray(counter, ref size, out _, null)
                .EnsureStatus(
                    PdhStatus.PDH_MORE_DATA,
                    nameof(PdhExports.PdhGetRawCounterArray));

            return size;
        }

        public static unsafe int EstimateFormattedCounterArraySize(this PdhCounter counter)
        {
            var size = 0;
            PdhExports
                .PdhGetFormattedCounterArray(counter, DefaultFmt, ref size, out _, null)
                .EnsureStatus(
                    PdhStatus.PDH_CSTATUS_VALID_DATA,
                    PdhStatus.PDH_MORE_DATA,
                    nameof(PdhExports.PdhGetRawCounterArray));

            return size;
        }
    }
}