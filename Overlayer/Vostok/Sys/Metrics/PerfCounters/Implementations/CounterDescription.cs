using Vostok.Sys.Metrics.PerfCounters.PDH;

namespace Vostok.Sys.Metrics.PerfCounters.Implementations
{
    internal struct CounterDescription<T>
    {
        public string Category;
        public string Counter;
        public string Instance;
        public PdhCounter PdhCounter;
        public SetValue<T> SetValue;
        public CounterReadMode Mode;

        public CounterDescription(string category, string counter, string instance)
            : this()
        {
            Category = category;
            Counter = counter;
            Instance = instance;
        }

        public CounterDescription(string category, string counter, SetValue<T> value, CounterReadMode mode)
            : this()
        {
            Category = category;
            Counter = counter;
            SetValue = value;
            Mode = mode;
        }
    }
}