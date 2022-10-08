using JetBrains.Annotations;

namespace Vostok.Metrics.Primitives.Gauge
{
    [PublicAPI]
    public class IntegerGaugeConfig : GaugeConfig
    {
        internal static readonly IntegerGaugeConfig Default = new IntegerGaugeConfig();

        /// <summary>
        /// If set to <c>true</c>, gauge value will be reset to initial value after each scrape.
        /// </summary>
        public bool ResetOnScrape { get; set; }

        /// <summary>
        /// Initial value of the gauge. Zero by default.
        /// </summary>
        public long InitialValue { get; set; }
        
        /// <summary>
        /// Whether or not to send initial value.
        /// </summary>
        public bool SendInitialValue { get; set; } = true;
    }
}