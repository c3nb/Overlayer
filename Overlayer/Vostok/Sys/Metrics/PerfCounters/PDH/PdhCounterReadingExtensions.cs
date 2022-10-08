using System;
using System.Collections.Generic;
using Vostok.Sys.Metrics.PerfCounters.Utilities;

namespace Vostok.Sys.Metrics.PerfCounters.PDH
{
    internal class Sample
    {
        public string Instance;
        public int Id;
        public double Value;
    }

    internal class InstancesCounter
    {
        private readonly Dictionary<string, int> instanceIndexes = new Dictionary<string, int>();

        public int GetAndIncrement(string name)
        {
            var value = instanceIndexes.TryGetValue(name, out var v) ? v : 0;
            instanceIndexes[name] = value + 1;
            return value;
        }

        public void Clear() => instanceIndexes.Clear();
    }

    internal static class PdhCounterReadingExtensions
    {
        public static unsafe void ReadFormattedValues(
            this PdhCounter counter,
            ArrayHolder arrayHolder,
            InstancesCounter instancesCounter,
            List<Sample> samples,
            out int size)
        {
            samples.Clear();
            instancesCounter.Clear();
            while (true)
            {
                size = counter.EstimateFormattedCounterArraySize();
                if (size == 0)
                    return;
                var buffer = arrayHolder.Get(size);
                fixed (byte* b = buffer)
                {
                    var ptr = (PDH_FMT_COUNTERVALUE_ITEM*) b;
                    var status = counter.GetFormattedCounterArray(ref size, out var items, ptr);

                    if (status == PdhStatus.PDH_CSTATUS_INVALID_DATA)
                        return;
                    if (status == PdhStatus.PDH_MORE_DATA)
                        continue;

                    status.EnsureSuccess(nameof(PdhExports.PdhGetFormattedCounterArray));

                    if (items * sizeof(PDH_FMT_COUNTERVALUE_ITEM) > buffer.Length)
                        throw new InvalidOperationException($"Buffer overflow check failed. ItemCount: {items}, ItemSize: {sizeof(PDH_FMT_COUNTERVALUE_ITEM)}, BufferSize: {buffer.Length}");

                    for (var i = 0; i < items; ++i)
                    {
                        var current = ptr + i;
                        var instanceName = new string(current->Name); // TODO: avoid allocation here
                        samples.Add(
                            new Sample
                            {
                                Instance = instanceName,
                                Value = current->FmtValue.DoubleValue,
                                Id = instancesCounter.GetAndIncrement(instanceName)
                            });
                    }

                    break;
                }
            }
        }
        public static unsafe void ReadRawValues(
            this PdhCounter counter,
            ArrayHolder arrayHolder,
            InstancesCounter instancesCounter,
            List<Sample> samples,
            out int size)
        {
            samples.Clear();
            instancesCounter.Clear();
            while (true)
            {
                size = counter.EstimateRawCounterArraySize();
                if (size == 0)
                    return;
                var buffer = arrayHolder.Get(size);
                fixed (byte* b = buffer)
                {
                    var ptr = (PDH_RAW_COUNTER_ITEM*) b;
                    var status = counter.GetRawCounterArray(ref size, out var items, ptr);

                    if (status == PdhStatus.PDH_CSTATUS_INVALID_DATA)
                        return;
                    if (status == PdhStatus.PDH_MORE_DATA)
                        continue;

                    status.EnsureSuccess(nameof(PdhExports.PdhGetFormattedCounterArray));

                    if (items * sizeof(PDH_RAW_COUNTER_ITEM) > buffer.Length)
                        throw new InvalidOperationException($"Buffer overflow check failed. ItemCount: {items}, ItemSize: {sizeof(PDH_FMT_COUNTERVALUE_ITEM)}, BufferSize: {buffer.Length}");

                    for (var i = 0; i < items; ++i)
                    {
                        var current = ptr + i;
                        var instanceName = new string(current->Name); // TODO: avoid allocation here
                        samples.Add(
                            new Sample
                            {
                                Instance = instanceName,
                                Value = current->RawValue.FirstValue,
                                Id = instancesCounter.GetAndIncrement(instanceName)
                            });
                    }

                    break;
                }
            }
        }
    }
}