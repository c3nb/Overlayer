using System;

namespace Vostok.Metrics.Scraping
{
    internal class ScrapableMetricConfig
    {
        public string Unit { get; set; }
        public TimeSpan? ScrapePeriod { get; set; }
        public bool ScrapeOnDispose { get; set; }
    }
}