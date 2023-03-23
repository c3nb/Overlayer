namespace Vostok.Sys.Metrics.PerfCounters
{
    public interface IPerformanceCounterFactory
    {
        IPerformanceCounterBuilder<T> Create<T>()
            where T : new();

        IPerformanceCounterBuilder Create();
    }

    public class PerformanceCounterFactory
    {
        public static IPerformanceCounterFactory Default = new PerformanceCounterFactoryInternal();
    }
}