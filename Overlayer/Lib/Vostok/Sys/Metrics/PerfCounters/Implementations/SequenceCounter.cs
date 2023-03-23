using System.Collections.Generic;
using Vostok.Sys.Metrics.PerfCounters.PDH;
using Vostok.Sys.Metrics.PerfCounters.Utilities;

namespace Vostok.Sys.Metrics.PerfCounters.Implementations
{
    internal class SequenceCounter : IPerformanceCounter<IEnumerable<Observation>>
    {
        private readonly CounterDescription<None>[] counters;
        private readonly ArrayHolder arrayHolder = new ArrayHolder();
        private readonly InstancesCounter instancesCounter = new InstancesCounter();
        private readonly List<Sample> samples = new List<Sample>();

        private volatile PdhQueryHandle query;

        public SequenceCounter(CounterDescription<None>[] counters)
        {
            this.counters = counters;
        }

        public IEnumerable<Observation> Observe()
        {
            if (query == null)
                Init();

            var collectStatus = query.CollectQueryData();

            if (collectStatus == PdhStatus.PDH_NO_DATA)
                yield break;

            collectStatus.EnsureSuccess(nameof(PdhExports.PdhCollectQueryData));

            foreach (var c in counters)
            {
                if (!c.PdhCounter.IsValid)
                    continue;

                c.PdhCounter.ReadFormattedValues(arrayHolder, instancesCounter, samples, out _);

                foreach (var sample in samples)
                    yield return new Observation(c.Category, c.Counter, sample.Instance, sample.Id, sample.Value);
            }
        }

        public void Dispose()
        {
            query?.Dispose();
        }

        private void Init()
        {
            query = PdhQueryHandle.OpenRealtime();
            for (var i = 0; i < counters.Length; ++i)
            {
                ref var c = ref counters[i];
                c.PdhCounter = query.AddCounter(c.Category, c.Counter, c.Instance, false);
            }
        }
    }
}