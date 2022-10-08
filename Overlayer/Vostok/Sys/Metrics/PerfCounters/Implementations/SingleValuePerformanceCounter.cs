using Vostok.Sys.Metrics.PerfCounters.Exceptions;
using Vostok.Sys.Metrics.PerfCounters.PDH;
using Vostok.Sys.Metrics.PerfCounters.Utilities;

namespace Vostok.Sys.Metrics.PerfCounters.Implementations
{
    internal class SingleValuePerformanceCounter<T> : IPerformanceCounter<T>
        where T : new()
    {
        private readonly SetCounterValueContext<T> context = new SetCounterValueContext<T>();
        private readonly CounterDescription<T>[] counters;
        private readonly string instanceName;
        private volatile bool firstObservation = true;
        private volatile PdhQueryHandle query;

        public SingleValuePerformanceCounter(CounterDescription<T>[] counters, string instanceName = null)
        {
            this.counters = counters;
            this.instanceName = instanceName;
        }

        public T Observe()
        {
            try
            {
                return RawObserve();
            }
            catch (PdhException e)
            {
                switch (e.Status)
                {
                    case PdhStatus.PDH_CALC_NEGATIVE_VALUE:
                    case PdhStatus.PDH_CALC_NEGATIVE_TIMEBASE:
                    case PdhStatus.PDH_CALC_NEGATIVE_DENOMINATOR:
                        Dispose();
                        query = null;
                        firstObservation = true;
                        return Factory.Create<T>();
                    default:
                        throw;
                }
            }
        }

        public void Dispose()
            => query?.Dispose();

        private T RawObserve()
        {
            if (query == null || query.IsInvalid)
                Init();

            Collect();

            try
            {
                context.Result = Factory.Create<T>();

                for (var i = 0; i < counters.Length; ++i)
                {
                    ref var counter = ref counters[i];
                    if (counter.Mode == CounterReadMode.RawValue)
                        counter.SetValue(context, GetRawValue(counter.PdhCounter).FirstValue);
                    else
                        counter.SetValue(context, GetValue(counter.PdhCounter, firstObservation));
                }

                return context.Result;
            }
            finally
            {
                firstObservation = false;
            }
        }

        private void Init()
        {
            query = PdhQueryHandle.OpenRealtime();

            for (var i = 0; i < counters.Length; ++i)
            {
                ref var counter = ref counters[i];
                counter.PdhCounter = query.AddCounter(counter.Category, counter.Counter, instanceName, true);
            }
        }

        private void Collect()
        {
            var status = query.CollectQueryData();
            if (status == PdhStatus.PDH_NO_DATA)
                throw new InvalidInstanceException(instanceName);

            status.EnsureSuccess(nameof(PdhExports.PdhCollectQueryData));
        }

        private double GetValue(PdhCounter counter, bool firstObservation)
        {
            var status = counter.GetFormattedValue(out var counterValue);

            switch (status)
            {
                case PdhStatus.PDH_INVALID_DATA when counterValue.CStatus == PdhStatus.PDH_CSTATUS_NO_INSTANCE:
                    throw new InvalidInstanceException(instanceName);
                case PdhStatus.PDH_INVALID_DATA when firstObservation:
                case PdhStatus.PDH_CALC_NEGATIVE_VALUE:
                case PdhStatus.PDH_CALC_NEGATIVE_TIMEBASE:
                case PdhStatus.PDH_CALC_NEGATIVE_DENOMINATOR:
                    return 0;
            }

            status.EnsureSuccess(nameof(PdhExports.PdhGetFormattedCounterValue));
            return counterValue.DoubleValue;
        }

        private RawValue GetRawValue(PdhCounter counter)
        {
            var status = counter.GetRawValue(out var counterValue);

            switch (status)
            {
                case PdhStatus.PDH_INVALID_DATA when counterValue.CStatus == PdhStatus.PDH_CSTATUS_NO_INSTANCE:
                    throw new InvalidInstanceException(instanceName);
                case PdhStatus.PDH_INVALID_DATA:
                case PdhStatus.PDH_CALC_NEGATIVE_VALUE:
                case PdhStatus.PDH_CALC_NEGATIVE_TIMEBASE:
                case PdhStatus.PDH_CALC_NEGATIVE_DENOMINATOR:
                    return new RawValue(0);
            }

            status.EnsureSuccess(nameof(PdhExports.PdhGetFormattedCounterValue));
            return new RawValue(counterValue.FirstValue);
        }
    }
}