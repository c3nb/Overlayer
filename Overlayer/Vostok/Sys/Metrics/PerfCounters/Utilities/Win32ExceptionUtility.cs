using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Vostok.Sys.Metrics.PerfCounters.Utilities
{
    internal static class Win32ExceptionUtility
    {
        public static void Throw(int errorCode)
            => throw CreateException(errorCode);

        public static void Throw()
            => Throw(Marshal.GetLastWin32Error());

        public static Exception CreateException()
            => CreateException(Marshal.GetLastWin32Error());

        public static Exception CreateException(int errorCode)
        {
            var win32ExceptionWithBadMessage = new Win32Exception(errorCode);
            return new Win32Exception(errorCode, $"{win32ExceptionWithBadMessage.Message}. Error code: {errorCode} (0x{errorCode:X})");
        }
    }
}