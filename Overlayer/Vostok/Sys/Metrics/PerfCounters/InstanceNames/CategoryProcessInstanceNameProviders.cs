using System;
using Vostok.Sys.Metrics.PerfCounters.Utilities;

namespace Vostok.Sys.Metrics.PerfCounters.InstanceNames
{
    internal class CategoryProcessInstanceNameProviders : ICategoryProcessInstanceNameProviders
    {
        private readonly bool forDotNetCounters;

        private readonly Func<string> currentProcessInstanceNameProvider;

        internal CategoryProcessInstanceNameProviders(bool forDotNetCounters)
        {
            this.forDotNetCounters = forDotNetCounters;
            currentProcessInstanceNameProvider =
                ProcessInstanceNamesCache.Instance.ForPid(ProcessUtility.CurrentProcessId, forDotNetCounters);
        }

        public Func<string> ForPid(int pid)
            => pid == ProcessUtility.CurrentProcessId ? ForCurrentProcess() : ProcessInstanceNamesCache.Instance.ForPid(pid, forDotNetCounters);

        public Func<string> ForCurrentProcess()
            => currentProcessInstanceNameProvider;
    }
}