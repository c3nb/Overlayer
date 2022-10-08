using System;
using System.Collections.Generic;

namespace Vostok.Sys.Metrics.PerfCounters.Implementations
{
    internal interface IPerformanceCounterFactoryInternal : IPerformanceCounterFactory
    {
        IPerformanceCounter<T> Create<T>(Func<string> instanceNameProvider, CounterDescription<T>[] counters)
            where T : new();

        IPerformanceCounter<T> Create<T>(string instanceName, CounterDescription<T>[] counters)
            where T : new();

        IPerformanceCounter<T> Create<T>(CounterDescription<T>[] counters)
            where T : new();

        IPerformanceCounter<Observation<T>[]> CreateForMultipleInstances<T>(string instanceNameWildcard, CounterDescription<T>[] counters)
            where T : new();

        IPerformanceCounter<IEnumerable<Observation>> Create(CounterDescription<None>[] counters);
    }
}