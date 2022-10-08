using System;
using Vostok.Sys.Metrics.PerfCounters.Exceptions;
using Vostok.Sys.Metrics.PerfCounters.Implementations;

namespace Vostok.Sys.Metrics.PerfCounters.PDH
{
    internal static class PdhQueryHandleExtensions
    {
        public static PdhStatus CollectQueryData(this PdhQueryHandle query)
            => PdhExports.PdhCollectQueryData(query);

        public static PdhCounter AddCounter(this PdhQueryHandle query, string category, string counter, string instance, bool throwOnError)
        {
            var path = instance == null
                ? CounterPathFactory.Create(category, counter)
                : CounterPathFactory.Create(category, counter, instance);

            var status = PdhExports.PdhAddEnglishCounter(query, path, IntPtr.Zero, out var pdhCounter);

            if (status == PdhStatus.PDH_CSTATUS_VALID_DATA)
                return pdhCounter;

            if (!throwOnError)
                return default;

            if (status == PdhStatus.PDH_CSTATUS_NO_COUNTER)
                throw new InvalidCounterException(category, counter);

            if (status == PdhStatus.PDH_CSTATUS_NO_OBJECT)
                throw new InvalidCategoryException(category);

            status.EnsureSuccess(nameof(PdhExports.PdhAddEnglishCounter), $"Category: {category}, Counter: {counter}.");
            return default;
        }
    }
}