using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace Vostok.Commons.Helpers.Windows
{
    [PublicAPI]
    internal static class ProcessPriorityHelper
    {
        /// <summary>
        /// http://msdn.microsoft.com/en-us/library/windows/desktop/hh448387(v=vs.85).aspx
        /// </summary>
        public static void SetMemoryPriority(ProcessMemoryPriority priority)
        {
            var priorityInfo = new MEMORY_PRIORITY_INFORMATION
            {
                MemoryPriority = (uint)priority
            };

            var priorityInfoLength = Marshal.SizeOf(priorityInfo);
            var priorityInfoPointer = Marshal.AllocHGlobal(priorityInfoLength);

            Marshal.StructureToPtr(priorityInfo, priorityInfoPointer, false);

            NtSetInformationProcess(Process.GetCurrentProcess().Handle, PROCESS_INFORMATION_CLASS.ProcessMemoryPriority, priorityInfoPointer, (uint)priorityInfoLength);
        }

        /// <summary>
        /// http://msdn.microsoft.com/ru-ru/library/windows/desktop/ms685100(v=vs.85).aspx
        /// </summary>
        public static void SetProcessPriorityClass(ProcessPriorityClass priorityClass)
        {
            Process.GetCurrentProcess().PriorityClass = priorityClass;
        }

        [DllImport("ntdll.dll", SetLastError = true)]
        private static extern bool NtSetInformationProcess(IntPtr hProcess, PROCESS_INFORMATION_CLASS ProcessInformationClass, IntPtr ProcessInformation, uint ProcessInformationSize);

        private enum PROCESS_INFORMATION_CLASS
        {
            ProcessMemoryPriority = 39,
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MEMORY_PRIORITY_INFORMATION
        {
            public uint MemoryPriority;
        }
    }
}