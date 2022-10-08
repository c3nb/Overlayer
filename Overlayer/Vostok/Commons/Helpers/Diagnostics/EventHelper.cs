using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using JetBrains.Annotations;

namespace Vostok.Commons.Helpers.Diagnostics;

[PublicAPI]
internal static class EventHelper
{
    /// <summary>
    /// <para>Get as-is <b>'Mean'</b> value from <b>EventCounter and PollingCounter</b>.</para>
    /// </summary>
    public static bool TryGetMeanCounterValue([CanBeNull] IDictionary<string, object> payload, out long value)
    {
        // NOTE: We use mean value from EventCounter and PollingCounter.
        // NOTE: See https://github.com/dotnet/runtime/blob/main/docs/design/features/event-counter.md#canonicalizing-a-single-value-output-per-counter for details.

        value = 0;
        if (payload == null || !payload.TryGetValue("Mean", out var mean))
            return false;

        value = Convert.ToInt64(mean);
        return true;
    }

    /// <summary>
    /// <para>Get <b>'Computed'</b> value from <b>IncrementingEventCounter and IncrementingPollingCounter</b>.</para>
    /// <para>Counter value computed by reading the 'Value' and multiplying it by the number of measurement intervals per minute.</para>
    /// </summary>
    public static bool TryGetIncrementingCounterValue([CanBeNull] IDictionary<string, object> payload, out long value)
    {
        // NOTE: We use computable value from IncrementingEventCounter and IncrementingPollingCounter.
        // NOTE: See https://github.com/dotnet/runtime/blob/main/docs/design/features/event-counter.md#canonicalizing-a-single-value-output-per-counter for details.

        value = 0;
        if (payload == null || !payload.TryGetValue("Increment", out var increment) || !payload.TryGetValue("IntervalSec", out var interval))
            return false;

        var counterValue = Convert.ToInt64(increment);
        var counterInterval = Convert.ToInt64(interval);

        value = 60 / counterInterval * counterValue;
        return true;
    }

    /// <summary>
    /// <para>Get counter payload from <see cref="EventWrittenEventArgs">eventData</see>.</para>
    /// <para>Remember counter payload structure looks like a <see cref="IDictionary{TKey,TValue}"/>, but for normal event it is a <see cref="IReadOnlyCollection{T}"/>.</para>
    /// </summary>
    public static bool TryGetCounterPayload([NotNull] EventWrittenEventArgs eventData, out IDictionary<string, object> payload)
    {
        // NOTE: See https://github.com/dotnet/runtime/blob/main/docs/design/features/event-counter.md#api-design for examples payload structures.

        payload = default;
        if (eventData.Payload?.Count <= 0
            || eventData.Payload?[0] is not IDictionary<string, object> data)
            return false;

        payload = data;
        return true;
    }

    /// <summary>
    /// <para>Get event counter value regardless of counter type.</para>
    /// <para>Use <see cref="TryGetMeanCounterValue"/> and <see cref="TryGetIncrementingCounterValue"/>.</para>
    /// </summary>
    public static bool TryGetCounterValue([NotNull] EventWrittenEventArgs eventData, string counterName, out long value)
    {
        value = 0;
        if (!TryGetCounterPayload(eventData, out var data)
            || !data!.TryGetValue("Name", out var n)
            || n is not string name
            || name != counterName) return false;

        return TryGetMeanCounterValue(data, out value) || TryGetIncrementingCounterValue(data, out value);
    }

    /// <summary>
    /// <para>Get event payload value by specified index.</para>>
    /// </summary>
    public static bool TryGetEventValue([NotNull] EventWrittenEventArgs eventData, int payloadIndex, out object payload)
    {
        payload = default;

        if (eventData.Payload == null
            || eventData.Payload.Count <= payloadIndex)
            return false;

        payload = eventData.Payload[payloadIndex];
        return true;
    }
}