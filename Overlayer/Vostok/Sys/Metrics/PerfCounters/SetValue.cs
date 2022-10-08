namespace Vostok.Sys.Metrics.PerfCounters
{
    public delegate void SetValue<T>(SetCounterValueContext<T> c, double value);
}