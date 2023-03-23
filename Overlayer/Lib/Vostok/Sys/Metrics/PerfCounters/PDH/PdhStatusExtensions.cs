namespace Vostok.Sys.Metrics.PerfCounters.PDH
{
    internal static class PdhStatusExtensions
    {
        public static bool IsLargerBufferRequired(this PdhStatus status)
            => status == PdhStatus.PDH_MORE_DATA;

        public static void EnsureSuccess(this PdhStatus status, string method, string message = "")
        {
            if (status != 0)
                FailWithError(status, method, message);
        }

        public static void EnsureStatus(this PdhStatus status, PdhStatus successfulStatus, string method,
            string message = "")
        {
            if (status != successfulStatus)
                FailWithError(status, method, message);
        }

        public static void EnsureStatus(this PdhStatus status, PdhStatus successfulStatus1, PdhStatus successfulStatus2,
            string method, string message = "")
        {
            if (status != successfulStatus1 && status != successfulStatus2)
                FailWithError(status, method, message);
        }

        private static void FailWithError(PdhStatus error, string function, string message)
            => throw CreateException(error, function, message);

        private static PdhException CreateException(PdhStatus error, string function, string message) =>
            new PdhException(function, error, message);
    }
}