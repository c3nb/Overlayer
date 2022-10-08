using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Vostok.Commons.Helpers.Disposable;
using Vostok.Commons.Helpers.Extensions;

namespace Vostok.Metrics.Scraping
{
    internal class ScrapeScheduler
    {
        private readonly IMetricEventSender sender;
        private readonly Action<Exception> errorCallback;
        private readonly bool scrapeOnDispose;
        private readonly CancellationTokenSource cancellation;
        private readonly ConcurrentDictionary<TimeSpan, ScrapableMetrics> scrapableMetrics;
        private readonly ConcurrentDictionary<TimeSpan, (Scraper scraper, Task runJob)> scrapersWithJobs;

        public ScrapeScheduler(IMetricEventSender sender, Action<Exception> errorCallback, bool scrapeOnDispose = false)
        {
            this.sender = sender;
            this.errorCallback = errorCallback;
            this.scrapeOnDispose = scrapeOnDispose;

            cancellation = new CancellationTokenSource();
            scrapableMetrics = new ConcurrentDictionary<TimeSpan, ScrapableMetrics>();
            scrapersWithJobs = new ConcurrentDictionary<TimeSpan, (Scraper scraper, Task runJob)>();
        }

        public IDisposable Register(IScrapableMetric metric, TimeSpan scrapePeriod)
        {
            if (scrapePeriod <= TimeSpan.Zero)
                throw new ArgumentOutOfRangeException(nameof(scrapePeriod), scrapePeriod, "Scrape period must be positive.");

            var (metrics, created) = ObtainMetricsForPeriod(scrapePeriod);

            metrics.Add(metric);

            if (created)
            {
                var scraper = new Scraper(sender, errorCallback);
                var job = Task.Run(() => scraper.RunAsync(metrics, scrapePeriod, cancellation.Token));
                scrapersWithJobs[scrapePeriod] = (scraper, job);
            }

            return new ActionDisposable(
                () =>
                {
                    metrics.Remove(metric);

                    if (scrapersWithJobs.TryGetValue(scrapePeriod, out var scraperWithJob))
                        scraperWithJob.scraper.WaitForIterationEnd().GetAwaiter().GetResult();

                    ScrapeOnDispose(metric);
                });
        }

        public void Dispose()
        {
            cancellation.Cancel();

            Task.WhenAll(scrapersWithJobs.Values.Select(x => x.runJob.SilentlyContinue()))
                .GetAwaiter()
                .GetResult();

            ScrapeOnDispose();

            scrapersWithJobs.Clear();
            scrapableMetrics.Clear();
        }

        private void ScrapeOnDispose()
        {
            if (!scrapeOnDispose)
                return;

            new Scraper(sender, errorCallback)
                .ScrapeOnce(scrapableMetrics.SelectMany(m => m.Value));
        }

        private void ScrapeOnDispose(IScrapableMetric metric)
        {
            if (!scrapeOnDispose)
                return;

            new Scraper(sender, errorCallback)
                .ScrapeOnce(new[] {metric});
        }

        private (ScrapableMetrics metrics, bool created) ObtainMetricsForPeriod(TimeSpan scrapePeriod)
        {
            if (scrapableMetrics.TryGetValue(scrapePeriod, out var metrics))
                return (metrics, false);

            var newMetrics = new ScrapableMetrics();

            if (scrapableMetrics.TryAdd(scrapePeriod, newMetrics))
                return (newMetrics, true);

            return (scrapableMetrics[scrapePeriod], false);
        }
    }
}