using System;
using JetBrains.Annotations;
using Vostok.Metrics.Models;

namespace Vostok.Metrics.Primitives.Counter
{
    /// <summary>
    /// <para>
    /// Counter represents a value that can only increase. Counters with the same tags are summed up server side.
    /// </para>
    /// <para>
    /// Each <see cref="Add"/> call increases the value of an internal in-memory counter.
    /// </para>
    /// <para>
    /// Counter primitive then periodically produces a <see cref="MetricEvent"/> containing the current of value of that counter and resets it to zero.
    /// </para>
    /// <remarks>
    /// <para>
    /// To create a Counter, use <see cref="CounterFactoryExtensions">extensions</see> for <see cref="IMetricContext"/>.
    /// </para>
    /// </remarks>
    /// <example>
    /// <para>
    /// You can use a Counter to represent the number of newly-registered users in your app.
    /// Every replica of your API creates a Counter named "registered-users".
    /// When API replica registers a new user, it calls <see cref="ICounterExtensions.Increment"/>.
    /// Values from all replicas are summed up and the single value is saved to a permanent storage.
    /// </para>
    /// </example>
    /// </summary>
    [PublicAPI]
    public interface ICounter
    {
        /// <summary>
        /// Adds given non-negative <paramref name="value"/> to the counter.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Passed <paramref name="value"/> was negative.</exception>
        void Add(long value);
    }
}