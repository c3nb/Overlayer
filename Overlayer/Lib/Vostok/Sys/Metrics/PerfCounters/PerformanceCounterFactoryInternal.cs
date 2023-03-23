using System;
using System.Collections.Generic;
using Vostok.Sys.Metrics.PerfCounters.Builder;
using Vostok.Sys.Metrics.PerfCounters.Implementations;

namespace Vostok.Sys.Metrics.PerfCounters
{
    internal class PerformanceCounterFactoryInternal : IPerformanceCounterFactoryInternal
    {
        public IPerformanceCounterBuilder<T> Create<T>()
            where T : new() => new PerformanceCounterBuilder<T>(this);

        public IPerformanceCounterBuilder Create() => new PerformanceCounterBuilder(this);

        public IPerformanceCounter<T> Create<T>(Func<string> instanceNameProvider, CounterDescription<T>[] counters)
            where T : new() => new DynamicPerformanceCounter<T>(name => Create(name, counters), instanceNameProvider);

        public IPerformanceCounter<T> Create<T>(string instanceName, CounterDescription<T>[] counters)
            where T : new() => new SingleValuePerformanceCounter<T>(counters, instanceName);

        public IPerformanceCounter<T> Create<T>(CounterDescription<T>[] counters)
            where T : new() => new SingleValuePerformanceCounter<T>(counters);

        public IPerformanceCounter<Observation<T>[]> CreateForMultipleInstances<T>(string instanceNameWildcard, CounterDescription<T>[] counters)
            where T : new() => new MultiValuePerformanceCounter<T>(counters, instanceNameWildcard);

        public IPerformanceCounter<IEnumerable<Observation>> Create(CounterDescription<None>[] counters)
            => new SequenceCounter(counters);
    }
}