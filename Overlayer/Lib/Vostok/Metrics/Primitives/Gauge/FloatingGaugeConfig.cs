using JetBrains.Annotations;

namespace Vostok.Metrics.Primitives.Gauge
{
    [PublicAPI]
    public class FloatingGaugeConfig : GaugeConfig
    {
        internal static readonly FloatingGaugeConfig Default = new FloatingGaugeConfig();

        /// <summary>
        /// If set to <c>true</c>, gauge value will be reset to initial value after each scrape.
        /// </summary>
        public bool ResetOnScrape { get; set; }

        /// <summary>
        /// Initial value of the gauge. Zero by default.
        /// </summary>
        public double InitialValue { get; set; }
        
        /// <summary>
        /// Whether or not to send initial value.
        /// </summary>
        public bool SendInitialValue { get; set; } = true;
    }
}