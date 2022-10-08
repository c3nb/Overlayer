using System;
using System.Runtime.InteropServices;

namespace Vostok.Commons.Helpers.Windows.WinApi
{
    internal class Kernel32
    {
        private const string DllName = "kernel32.dll";

        [DllImport(DllName, SetLastError = true)]
        public static extern IntPtr CreateJobObject(IntPtr lpJobAttributes, IntPtr lpName);

        [DllImport(DllName, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseHandle(IntPtr hObject);

        [DllImport(DllName, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool AssignProcessToJobObject(IntPtr hJob, IntPtr hProcess);

        [DllImport(DllName, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetInformationJobObject(
            IntPtr hJob,
            JOBOBJECTINFOCLASS JobObjectInfoClass,
            ref JOBOBJECT_EXTENDED_LIMIT_INFORMATION lpJobObjectInfo,
            uint cbJobObjectInfoLength);

        [DllImport(DllName, SetLastError = true)]
        public static extern bool GetExitCodeProcess(
            [In] IntPtr hProcess,
            [Out] out int lpExitCode
        );
    }
}