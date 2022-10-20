using Overlayer.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Vostok.Sys.Metrics.PerfCounters;
using Vostok.Sys.Metrics.PerfCounters.Builder;

namespace Overlayer.Tags.Global
{
    public static class Performance
    {
        public static int ProcessorCount;
        public static ulong MemoryMBytes;
        public static double CpuUsage;
        public static double TotalCpuUsage;
        public static double MemoryUsage;
        public static double TotalMemoryUsage;
        public static ulong MemoryUsageMBytes;
        public static ulong TotalMemoryUsageMBytes;
        public static Thread UpdateThread;
        public static bool Initialized { get; private set; }
        public static void Init()
        {
            if (Initialized) return;
            ProcessorCount = Environment.ProcessorCount;
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Main.Logger.Log("Performance Monitoring Tag Is Not Supported At MacOS Or Linux.");
                Initialized = true;
                return;
            }
            var curProc = Process.GetCurrentProcess();
            MemoryMBytes = MemoryStatus.GetMemoryStatus().TotalPhysicalMemorySize / 1048576;
            var cpu = PerformanceCounterFactory.Default.CreateCounter("Process", "% Processor Time", curProc.ProcessName);
            var mem = PerformanceCounterFactory.Default.CreateCounter("Process", "Working Set", curProc.ProcessName);
            var totCpu = PerformanceCounterFactory.Default.CreateCounter("Processor", "% Processor Time", "_Total");
            var totMem = PerformanceCounterFactory.Default.CreateCounter("Memory", "Available MBytes");
            UpdateThread = new Thread(() =>
            {
                while (true)
                {
                    CpuUsage = cpu.Observe() / ProcessorCount;
                    TotalCpuUsage = totCpu.Observe();
                    var memUsage = mem.Observe() / 1048576;
                    MemoryUsageMBytes = (ulong)memUsage;
                    var totMemUsage = MemoryMBytes - totMem.Observe();
                    TotalMemoryUsageMBytes = (ulong)totMemUsage;
                    MemoryUsage = memUsage / MemoryMBytes * 100d;
                    TotalMemoryUsage = totMemUsage / MemoryMBytes * 100d;
                    Thread.Sleep(Settings.Instance.PerfStatUpdateRate);
                }
            });
            UpdateThread.Start();
            Initialized = true;
        }

        [Tag("ProcessorCount")]
        public static double ProcessorCount_() => ProcessorCount;
        [Tag("MemoryGBytes")]
        public static double MemoryGBytes_(double digits = -1) => (MemoryMBytes / (1024f * 1024f * 1024f)).Round(digits);
        [Tag("CpuUsage")]
        public static double CpuUsage_(double digits = -1) => CpuUsage.Round(digits);
        [Tag("TotalCpuUsage")]
        public static double TotalCpuUsage_(double digits = -1) => TotalCpuUsage.Round(digits);
        [Tag("MemoryUsage")]
        public static double MemoryUsage_(double digits = -1) => MemoryUsage.Round(digits);
        [Tag("TotalMemoryUsage")]
        public static double TotalMemoryUsage_(double digits = -1) => TotalMemoryUsage.Round(digits);
        [Tag("MemoryUsageGBytes")]
        public static double MemoryUsageGBytes_(double digits = -1) => (MemoryUsageMBytes / 1024f).Round(digits);
        [Tag("TotalMemoryUsageGBytes")]
        public static double TotalMemoryUsageGBytes_(double digits = -1) => (TotalMemoryUsageMBytes / 1024f).Round(digits);
        [StructLayout(LayoutKind.Sequential)]
        public class MemoryStatus
        {
            public uint Length = (uint)Marshal.SizeOf<MemoryStatus>();
            public uint MemoryLoad;
            public ulong TotalPhysicalMemorySize;
            public ulong AvailablePhysicalMemorySize;
            public ulong TotalPageFileSize;
            public ulong AvailablePageFileSize;
            public ulong TotalVirtualMemorySize;
            public ulong AvailableVirtualMemorySize;
            public ulong AvailableExtendedVirtualMemorySize;
            [DllImport("kernel32.dll", EntryPoint = "GlobalMemoryStatusEx")]
            static extern bool GetMemoryStatus_([In, Out] MemoryStatus lpBuffer);
            public static MemoryStatus GetMemoryStatus()
            {
                MemoryStatus status = new MemoryStatus();
                GetMemoryStatus_(status);
                return status;
            }
        }
    }
}
