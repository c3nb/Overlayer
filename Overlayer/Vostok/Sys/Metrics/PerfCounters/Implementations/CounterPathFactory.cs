namespace Vostok.Sys.Metrics.PerfCounters.Implementations
{
    internal static class CounterPathFactory
    {
        public static string Create(string categoryName, string counterName, string instanceName)
            => $@"\{categoryName}({instanceName})\{counterName}";

        public static string Create(string categoryName, string counterName)
            => $@"\{categoryName}\{counterName}";

        public static string CreateForAllInstances(string categoryName, string counterName)
            => $@"\{categoryName}(*)\{counterName}";
    }
}