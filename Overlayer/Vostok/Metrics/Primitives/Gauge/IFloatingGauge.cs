using JetBrains.Annotations;

namespace Vostok.Metrics.Primitives.Gauge
{
    /// <inheritdoc cref="GaugeDocumentation"/>
    [PublicAPI]
    public interface IFloatingGauge
    {
        /// <summary>
        /// Sets current gauge's value to given <paramref name="value"/>.
        /// </summary>
        void Set(double value);

        /// <summary>
        /// Adds given <paramref name="value"/> to current gauge value.
        /// </summary>
        void Add(double value);

        /// <summary>
        /// Substracts given <paramref name="value"/> from current gauge value.
        /// </summary>
        void Substract(double value);

        /// <summary>
        /// <para>This method will <see cref="Set"/> given <paramref name="value"/> if it's greater than current one.</para>
        /// <para>This is useful to track max values.</para>
        /// </summary>
        void TryIncreaseTo(double value);

        /// <summary>
        /// <para>This method will <see cref="Set"/> given <paramref name="value"/> if it's lesser than current one.</para>
        /// <para>This is useful to track min values.</para>
        /// </summary>
        void TryReduceTo(double value);
    }
}