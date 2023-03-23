using System;

namespace Vostok.Sys.Metrics.PerfCounters.PDH
{
    internal class PdhException : InvalidOperationException
    {
        public PdhException(string function, PdhStatus status, string message = "") : base(
            GetExceptionInfo(function, status, message)) => Status = status;

        public PdhStatus Status { get; }

        private static string GetExceptionInfo(string function, PdhStatus status, string message)
        {
            var info =
                $"Pdh function {function} failed with code {(int) status:x8} ({Enum.GetName(typeof(PdhStatus), status)})";

            return string.IsNullOrEmpty(message)
                ? info
                : $"{message} {info}";
        }
    }
}