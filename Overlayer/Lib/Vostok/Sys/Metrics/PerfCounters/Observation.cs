namespace Vostok.Sys.Metrics.PerfCounters
{
    public readonly struct Observation
    {
        public readonly string Category;
        public readonly string Counter;
        public readonly string Instance;
        public readonly int Id;
        public readonly double Value;

        public Observation(string category, string counter, string instance, int id, double value)
        {
            Category = category;
            Counter = counter;
            Instance = instance;
            Id = id;
            Value = value;
        }
    }

    public readonly struct Observation<T>
    {
        public readonly string Instance;
        public readonly int Id;
        public readonly T Value;

        public Observation(string instance, int id, T value)
        {
            Instance = instance;
            Id = id;
            Value = value;
        }
    }
}