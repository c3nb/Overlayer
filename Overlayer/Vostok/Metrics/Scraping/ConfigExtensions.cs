using Vostok.Metrics.Primitives.Counter;
using Vostok.Metrics.Primitives.Gauge;
using Vostok.Metrics.Primitives.Timer;

namespace Vostok.Metrics.Scraping
{
    // NOTE (tsup): We do not inherit configs from ScrapableMetricConfig to keep back-compatibility. 
    internal static class ConfigExtensions
    {
        public static ScrapableMetricConfig ToScrapableMetricConfig(this CounterConfig config)
        {
            return new ScrapableMetricConfig
            {
                ScrapePeriod = config.ScrapePeriod,
                ScrapeOnDispose = config.ScrapeOnDispose,
                Unit = config.Unit
            };
        }

        public static ScrapableMetricConfig ToScrapableMetricConfig(this HistogramConfig config)
        {
            return new ScrapableMetricConfig
            {
                ScrapePeriod = config.ScrapePeriod,
                ScrapeOnDispose = config.ScrapeOnDispose,
                Unit = config.Unit
            };
        }

        public static ScrapableMetricConfig ToScrapableMetricConfig(this GaugeConfig config)
        {
            return new ScrapableMetricConfig
            {
                ScrapePeriod = config.ScrapePeriod,
                ScrapeOnDispose = config.ScrapeOnDispose,
                Unit = config.Unit
            };
        }

        public static ScrapableMetricConfig ToScrapableMetricConfig(this SummaryConfig config)
        {
            return new ScrapableMetricConfig
            {
                ScrapePeriod = config.ScrapePeriod,
                ScrapeOnDispose = config.ScrapeOnDispose,
                Unit = config.Unit
            };
        }
    }
}