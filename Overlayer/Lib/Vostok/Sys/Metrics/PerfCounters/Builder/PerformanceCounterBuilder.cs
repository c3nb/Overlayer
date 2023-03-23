using System;
using System.Collections.Generic;
using Vostok.Sys.Metrics.PerfCounters.Implementations;

namespace Vostok.Sys.Metrics.PerfCounters.Builder
{
    internal class PerformanceCounterBuilder<T> : IPerformanceCounterBuilder<T>
        where T : new()
    {
        private readonly IPerformanceCounterFactoryInternal factory;
        private readonly List<CounterDescription<T>> counters = new List<CounterDescription<T>>();

        public PerformanceCounterBuilder(IPerformanceCounterFactoryInternal factory)
        {
            this.factory = factory;
        }

        public IPerformanceCounterBuilder<T> AddCounter(string category, string counter, SetValue<T> setValue)
        {
            var rawValue = false; // TODO: optimize some counters by raw value reading
            
            counters.Add(
                new CounterDescription<T>
                {
                    Category = category,
                    Counter = counter,
                    Mode = rawValue ? CounterReadMode.RawValue : CounterReadMode.FormattedValue,
                    SetValue = setValue
                });

            return this;
        }

        public IPerformanceCounter<T> Build()
            => factory.Create(counters.ToArray());

        public IPerformanceCounter<T> Build(string instanceName)
            => factory.Create(instanceName, counters.ToArray());

        public IPerformanceCounter<T> Build(Func<string> instanceNameProvider)
            => factory.Create(instanceNameProvider, counters.ToArray());

        public IPerformanceCounter<Observation<T>[]> BuildForMultipleInstances(string instanceNameFilter)
            => factory.CreateForMultipleInstances(instanceNameFilter, counters.ToArray());
    }
}