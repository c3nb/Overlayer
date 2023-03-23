using System;
using System.Collections.Generic;
using Vostok.Sys.Metrics.PerfCounters.Utilities;

namespace Vostok.Sys.Metrics.PerfCounters.InstanceNames
{
    internal class ProcessInstanceNamesCache
    {
        public static readonly ProcessInstanceNamesCache Instance = new ProcessInstanceNamesCache();
        private readonly TimeCache<Dictionary<int, string>> dotNetCache = new TimeCache<Dictionary<int, string>>(
            () => BuildCache(InstanceNameUtility.DotNet),
            () => TimeSpan.FromMilliseconds(500));

        private readonly TimeCache<Dictionary<int, string>> globalCache = new TimeCache<Dictionary<int, string>>(
            () => BuildCache(InstanceNameUtility.Process),
            () => TimeSpan.FromMilliseconds(500));

        public Func<string> ForPid(int pid, bool forDotNetCounters)
        {
            if (forDotNetCounters)
                return () => GetForPidDotNet(pid);
            return () => GetForPid(pid);
        }

        internal void EvictCaches()
        {
            dotNetCache.Evict();
            globalCache.Evict();
        }

        private static Dictionary<int, string> BuildCache(InstanceNameUtility utility)
        {
            try
            {
                return utility.ObtainInstanceNames();
            }
            catch (Exception)
            {
                //TODO: logging
                return new Dictionary<int, string>();
            }
        }

        private string GetForPidDotNet(int pid)
        {
            if (dotNetCache.GetValue().TryGetValue(pid, out var instanceId))
                return instanceId;
            ProcessUtility.EnsureProcessIsRunning(pid);
            return null;
        }

        private string GetForPid(int pid)
        {
            if (globalCache.GetValue().TryGetValue(pid, out var instanceId))
                return instanceId;
            ProcessUtility.EnsureProcessIsRunning(pid);
            globalCache.Evict();
            if (!globalCache.GetValue().TryGetValue(pid, out instanceId))
                throw new InvalidOperationException($"Process with pid {pid} exists, but not found in cache");
            return instanceId;
        }
    }
}