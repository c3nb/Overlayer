using System.Collections.Generic;
using Vostok.Sys.Metrics.PerfCounters.PDH;
using Vostok.Sys.Metrics.PerfCounters.Utilities;

namespace Vostok.Sys.Metrics.PerfCounters.Implementations
{
    internal class MultiValuePerformanceCounter<T> : IPerformanceCounter<Observation<T>[]>
        where T : new()
    {
        private readonly string instanceNameWildcard;
        private readonly CounterDescription<T>[] counters;
        private readonly Dictionary<(string name, int id), SetCounterValueContext<T>> contexts
            = new Dictionary<(string, int), SetCounterValueContext<T>>();
        private readonly ArrayHolder arrayHolder = new ArrayHolder();
        private readonly InstancesCounter instancesCounter = new InstancesCounter();
        private readonly List<Sample> samples = new List<Sample>();

        private volatile PdhQueryHandle query;

        public MultiValuePerformanceCounter(CounterDescription<T>[] counters, string instanceNameWildcard)
        {
            this.instanceNameWildcard = instanceNameWildcard;
            this.counters = counters;
        }

        public Observation<T>[] Observe()
        {
            if (query == null || query.IsInvalid)
                Init();

            Collect();

            contexts.Clear();

            for (var i = 0; i < counters.Length; ++i)
            {
                ref var counter = ref counters[i];

                if (!counter.PdhCounter.IsValid)
                    continue;

                if (counter.Mode == CounterReadMode.FormattedValue)
                    counter.PdhCounter.ReadFormattedValues(arrayHolder, instancesCounter, samples, out _);
                else    
                    counter.PdhCounter.ReadRawValues(arrayHolder, instancesCounter, samples, out _);

                foreach (var sample in samples)
                {
                    var instance = (sample.Instance, sample.Id);

                    if (!contexts.TryGetValue(instance, out var ctx))
                    {
                        ctx = new SetCounterValueContext<T>();

                        contexts[instance] = ctx;
                        ctx.Result = Factory.Create<T>();
                    }

                    counter.SetValue(ctx, sample.Value);
                }
            }

            var result = new Observation<T>[contexts.Count];

            var index = 0;

            foreach (var kvp in contexts)
                result[index++] = new Observation<T>(kvp.Key.name, kvp.Key.id, kvp.Value.Result);

            return result;
        }

        public void Dispose()
            => query?.Dispose();

        private void Init()
        {
            query = PdhQueryHandle.OpenRealtime();
            for (var i = 0; i < counters.Length; ++i)
            {
                ref var counter = ref counters[i];
                counter.PdhCounter = query.AddCounter(counter.Category, counter.Counter, instanceNameWildcard, true);
            }
        }

        private void Collect()
        {
            var status = query.CollectQueryData();
            if (status == PdhStatus.PDH_NO_DATA)
                return;
            status.EnsureSuccess(nameof(PdhExports.PdhCollectQueryData));
        }
    }
}