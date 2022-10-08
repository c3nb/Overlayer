using JetBrains.Annotations;

namespace Vostok.Metrics.Primitives.Gauge
{
    /// <inheritdoc cref="GaugeDocumentation"/>
    [PublicAPI]
    public interface IIntegerGauge
    {
        /// <summary>
        /// Sets current gauge's value to given <paramref name="value"/>.
        /// </summary>
        void Set(long value);

        /// <summary>
        /// Adds given <paramref name="value"/> to current gauge value.
        /// </summary>
        void Add(long value);

        /// <summary>
        /// Substracts given <paramref name="value"/> from current gauge value.
        /// </summary>
        void Substract(long value);

        /// <summary>
        /// Increments current gauge value by one.
        /// </summary>
        void Increment();

        /// <summary>
        /// Decrements current gauge value by one.
        /// </summary>
        void Decrement();

        /// <summary>
        /// <para>This method will <see cref="Set"/> given <paramref name="value"/> if it's greater than current one.</para>
        /// <para>This is useful to track max values.</para>
        /// </summary>
        void TryIncreaseTo(long value);

        /// <summary>
        /// <para>This method will <see cref="Set"/> given <paramref name="value"/> if it's lesser than current one.</para>
        /// <para>This is useful to track min values.</para>
        /// </summary>
        void TryReduceTo(long value);
    }
}