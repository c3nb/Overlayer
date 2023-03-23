using System;

namespace Vostok.Sys.Metrics.PerfCounters.InstanceNames
{
    public interface ICategoryProcessInstanceNameProviders
    {
        Func<string> ForPid(int pid);
        Func<string> ForCurrentProcess();
    }
}