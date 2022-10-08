using System;
using Microsoft.Win32.SafeHandles;

namespace Vostok.Sys.Metrics.PerfCounters.PDH
{
    internal readonly struct PdhCounter
    {
        private readonly IntPtr handle;
        public bool IsValid => handle != IntPtr.Zero;
    }

    internal class PdhQueryHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        private PdhQueryHandle()
            : base(true)
        {
        }

        public static PdhQueryHandle OpenRealtime()
        {
            PdhExports.PdhOpenQuery(null, IntPtr.Zero, out var handle).EnsureSuccess(nameof(PdhExports.PdhOpenQuery));
            return handle;
        }

        protected override bool ReleaseHandle() =>
            PdhExports.PdhCloseQuery(handle) == PdhStatus.PDH_CSTATUS_VALID_DATA;
    }
}