using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using Vostok.Commons.Helpers.Windows.WinApi;

namespace Vostok.Commons.Helpers.Windows
{
    [PublicAPI]
    internal class WindowsProcessKillJob : IDisposable
    {
        private readonly IntPtr jobHandle;

        public unsafe WindowsProcessKillJob()
        {
            jobHandle = Kernel32.CreateJobObject(IntPtr.Zero, IntPtr.Zero);
            if (jobHandle == IntPtr.Zero)
                throw new Win32Exception();
            try
            {
                var limitInformation = new JOBOBJECT_EXTENDED_LIMIT_INFORMATION
                {
                    BasicLimitInformation = new JOBOBJECT_BASIC_LIMIT_INFORMATION
                    {
                        LimitFlags = JOBOBJECT_BASIC_LIMIT_FLAGS.JOB_OBJECT_LIMIT_KILL_ON_JOB_CLOSE
                    }
                };

                var cb = sizeof(JOBOBJECT_EXTENDED_LIMIT_INFORMATION);

                if (!Kernel32.SetInformationJobObject(jobHandle, JOBOBJECTINFOCLASS.JobObjectExtendedLimitInformation, ref limitInformation, (uint)cb))
                    throw new Win32Exception();
            }
            catch (Exception)
            {
                Kernel32.CloseHandle(jobHandle);
                throw;
            }
        }

        public void Dispose()
        {
            Kernel32.CloseHandle(jobHandle);
        }

        public void AddProcess(Process process)
        {
            if (process == null)
                throw new ArgumentNullException(nameof(process));
            AddProcess(process.Handle);
        }

        private void AddProcess(IntPtr processHandle)
        {
            if (Kernel32.AssignProcessToJobObject(jobHandle, processHandle))
                return;
            var error = Marshal.GetLastWin32Error();
            if (!Kernel32.GetExitCodeProcess(processHandle, out var exitCode) || exitCode == ProcessExitCodes.STILL_ACTIVE)
                throw new Win32Exception(error);
        }
    }
}