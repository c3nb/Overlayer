using Overlayer.Core;
using System;
using System.Threading;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Vostok.Sys.Metrics.PerfCounters;
using Overlayer.Core.Tags;
using System.Runtime.ConstrainedExecution;

namespace Overlayer.Tags
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
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        public static void Init()
        {
            if (Initialized) return;
            ProcessorCount = Environment.ProcessorCount;
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                OverlayerDebug.Log("Performance Monitoring Tag Is Not Supported At MacOS Or Linux.");
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
                    Thread.Sleep(Main.Settings.PerfStatUpdateRate);
                }
            });
            UpdateThread.Start();
            Initialized = true;
        }

        [Tag("ProcessorCount", NotPlaying = true)]
        public static double ProcessorCount_() => ProcessorCount;
        [Tag("MemoryGBytes", NotPlaying = true)]
        public static double MemoryGBytes_(int digits = -1) => (MemoryMBytes / (1024d * 1024d * 1024d)).Round(digits);
        [Tag("CpuUsage", NotPlaying = true)]
        public static double CpuUsage_(int digits = -1) => CpuUsage.Round(digits);
        [Tag("TotalCpuUsage", NotPlaying = true)]
        public static double TotalCpuUsage_(int digits = -1) => TotalCpuUsage.Round(digits);
        [Tag("MemoryUsage", NotPlaying = true)]
        public static double MemoryUsage_(int digits = -1) => MemoryUsage.Round(digits);
        [Tag("TotalMemoryUsage", NotPlaying = true)]
        public static double TotalMemoryUsage_(int digits = -1) => TotalMemoryUsage.Round(digits);
        [Tag("MemoryUsageGBytes", NotPlaying = true)]
        public static double MemoryUsageGBytes_(int digits = -1) => (MemoryUsageMBytes / 1024d).Round(digits);
        [Tag("TotalMemoryUsageGBytes", NotPlaying = true)]
        public static double TotalMemoryUsageGBytes_(int digits = -1) => (TotalMemoryUsageMBytes / 1024d).Round(digits);
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
