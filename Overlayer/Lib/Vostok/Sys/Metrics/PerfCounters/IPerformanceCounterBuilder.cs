using System;
using System.Collections.Generic;
using Vostok.Sys.Metrics.PerfCounters.Implementations;

namespace Vostok.Sys.Metrics.PerfCounters
{
    public interface IPerformanceCounterBuilder<T>
    {
        IPerformanceCounterBuilder<T> AddCounter(string category, string counter, SetValue<T> setValue);

        IPerformanceCounter<T> Build();
        IPerformanceCounter<T> Build(string instance);
        IPerformanceCounter<T> Build(Func<string> instanceNameProvider);
        IPerformanceCounter<Observation<T>[]> BuildForMultipleInstances(string instancesFilter);
    }

    public interface IPerformanceCounterBuilder
    {
        IPerformanceCounterBuilder AddCounter(string category, string counter, string instancesFilter);

        IPerformanceCounter<IEnumerable<Observation>> Build();
    }

    internal class PerformanceCounterBuilder : IPerformanceCounterBuilder
    {
        private readonly IPerformanceCounterFactoryInternal factory;
        private readonly List<CounterDescription<None>> counters = new List<CounterDescription<None>>();

        public PerformanceCounterBuilder(IPerformanceCounterFactoryInternal factory)
        {
            this.factory = factory;
        }

        public IPerformanceCounterBuilder AddCounter(string category, string counter, string instancesFilter)
        {
            counters.Add(new CounterDescription<None> {Category = category, Counter = counter, Instance = instancesFilter});
            return this;
        }

        public IPerformanceCounter<IEnumerable<Observation>> Build() => factory.Create(counters.ToArray());
    }
}