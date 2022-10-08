using System;
using JetBrains.Annotations;

namespace Vostok.Metrics.Primitives.Gauge
{
    /// <inheritdoc cref="GaugeDocumentation"/>
    [PublicAPI]
    public interface IFuncGauge
    {
        void SetValueProvider([NotNull] Func<double> valueProvider);

        void SetValueProvider([NotNull] Func<double?> valueProvider);
    }
}