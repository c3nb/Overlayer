using JetBrains.Annotations;

namespace Vostok.Metrics.Primitives.Gauge
{
    [PublicAPI]
    public class FuncGaugeConfig : GaugeConfig
    {
        internal static readonly FuncGaugeConfig Default = new FuncGaugeConfig();
    }
}