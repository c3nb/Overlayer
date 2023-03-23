using JetBrains.Annotations;

namespace Vostok.Metrics.Primitives.Timer
{
    /// <summary>
    /// <para>Timers are used to measure duration of arbitrary operations.</para>
    /// <para>Timer implementations are expected to produce rate and latency metrics: ops per unit of time, duration quantiles.</para>
    /// <para>See <see cref="TimerFactoryExtensions"/> for a flavor of <see cref="ITimer"/> that performs no client side aggregation and produces an event for each measurement.</para>
    /// <para>See <see cref="HistogramFactoryExtensions"/> for a flavor of <see cref="ITimer"/> that preaggregates values into configured histogram buckets.</para>
    /// <para>See <see cref="SummaryFactoryExtensions"/> for a flavor of <see cref="ITimer"/> that computes approximate values with an online algorithm and is limited to a single process.</para>
    /// </summary>
    /// <example>
    /// Use <see cref="ITimerExtensions_Measurement">measurement extensions</see> to time operations in code:
    /// <code>
    /// using (timer.Measure())
    /// {
    ///     // perform operation
    /// }
    /// </code>
    /// </example>
    [PublicAPI]
    public interface ITimer
    {
        /// <summary>
        /// Returns the unit this timer's <see cref="Report"/> method assumes for passed values.
        /// </summary>
        [CanBeNull]
        string Unit { get; }

        /// <summary>
        /// Reports a measurement with given <paramref name="value"/> to the timer implementation.
        /// </summary>
        void Report(double value);
    }
}