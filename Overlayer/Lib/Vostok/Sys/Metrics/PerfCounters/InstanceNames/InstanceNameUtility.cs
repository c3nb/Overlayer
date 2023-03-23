using System.Collections.Generic;

namespace Vostok.Sys.Metrics.PerfCounters.InstanceNames
{
    internal class InstanceNameUtility
    {
        public static readonly InstanceNameUtility Process = new InstanceNameUtility("Process", "ID Process");
        public static readonly InstanceNameUtility DotNet = new InstanceNameUtility(".NET CLR Memory", "Process ID");

        private readonly IPerformanceCounter<Observation<int>[]> processIdPerfCounter;
        private readonly object sync = new object();
        private readonly Dictionary<int, CachedInstanceName> instanceNameCache = new Dictionary<int, CachedInstanceName>();
        private readonly List<int> valuesToRemove = new List<int>();

        private InstanceNameUtility(string category, string counter)
            => processIdPerfCounter = PerformanceCounterFactory
                .Default
                .Create<int>()
                .AddCounter(category, counter, (context, value) => context.Result = (int) value)
                .BuildForMultipleInstances("*");

        public Dictionary<int, string> ObtainInstanceNames()
        {
            lock (sync)
            {
                var observations = processIdPerfCounter.Observe();

                return BuildProcessIdToInstanceNameMap(observations);
            }
        }

        public void EvictCaches()
        {
            Process.EvictCaches();
            DotNet.EvictCaches();
        }

        private Dictionary<int, string> BuildProcessIdToInstanceNameMap(in Observation<int>[] observations)
        {
            var map = new Dictionary<int, string>(observations.Length);

            foreach (var observation in observations)
            {
                var instanceName = GetInstanceName(observation.Instance, observation.Value, observation.Id);
                map[observation.Value] = instanceName;
            }

            CleanupCache(map);

            return map;
        }

        private void CleanupCache(Dictionary<int, string> newMap)
        {
            valuesToRemove.Clear();
            foreach (var pid in instanceNameCache.Keys)
                if (!newMap.ContainsKey(pid))
                    valuesToRemove.Add(pid);
            foreach (var pid in valuesToRemove)
                instanceNameCache.Remove(pid);
        }

        private string GetInstanceName(string currentName, int pid, int instanceIndex)
        {
            // Use InstanceName strings cache to reduce memory usage. InstanceName can be reused if pid, instance index and process name haven't changed
            if (instanceNameCache.TryGetValue(pid, out var val) && val.InstanceIndex == instanceIndex && currentName == val.ProcessName)
                return val.InstanceName;
            var value = new CachedInstanceName
            {
                InstanceName = instanceIndex == 0 ? currentName : currentName + '#' + instanceIndex,
                InstanceIndex = instanceIndex,
                ProcessName = currentName
            };
            instanceNameCache[pid] = value;
            return value.InstanceName;
        }

        private struct CachedInstanceName
        {
            public string InstanceName;
            public string ProcessName;
            public int InstanceIndex;
        }
    }
}