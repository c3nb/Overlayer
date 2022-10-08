namespace Vostok.Sys.Metrics.PerfCounters.InstanceNames
{
    public static class InstanceNameProviders
    {
        public static readonly ICategoryProcessInstanceNameProviders Process = new CategoryProcessInstanceNameProviders(false);
        public static readonly ICategoryProcessInstanceNameProviders DotNet = new CategoryProcessInstanceNameProviders(true);
    }
}