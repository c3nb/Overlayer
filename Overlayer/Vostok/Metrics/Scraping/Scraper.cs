using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Vostok.Commons.Time;
using Vostok.Metrics.Models;
using Signal = Vostok.Commons.Threading.AsyncManualResetEvent;

namespace Vostok.Metrics.Scraping
{
    internal class Scraper
    {
        private static readonly TimeSpan RolloverWaitPause = TimeSpan.FromMilliseconds(1);
        private readonly Signal iterationEnd = new Signal(true);
        private readonly IMetricEventSender sender;
        private readonly Action<Exception> errorCallback;
        private readonly List<MetricEvent> metricEventsBuffer;

        public Scraper(IMetricEventSender sender, Action<Exception> errorCallback)
        {
            this.sender = sender;
            this.errorCallback = errorCallback;
            metricEventsBuffer = new List<MetricEvent>();
        }

        public async Task RunAsync(ScrapableMetrics metrics, TimeSpan period, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var initialTimestamp = Now;

                var delayToNextScrape = GetDelayToNextScrape(initialTimestamp, period);
                if (delayToNextScrape > TimeSpan.Zero)
                    await Task.Delay(delayToNextScrape, cancellationToken).ConfigureAwait(false);

                var thresholdTimestamp = initialTimestamp + delayToNextScrape;
                var scrapeTimestamp = await WaitForTimestampRollover(thresholdTimestamp, cancellationToken).ConfigureAwait(false);

                ScrapeOnce(metrics, scrapeTimestamp, cancellationToken);
            }
        }

        public void ScrapeOnce(IEnumerable<IScrapableMetric> metrics, DateTime? scrapeTimestamp = null, CancellationToken cancellationToken = default)
        {
            iterationEnd.Reset();
            Scrape(metrics, scrapeTimestamp, cancellationToken);
            iterationEnd.Set();

            foreach (var metricEvent in metricEventsBuffer)
            {
                if (cancellationToken.IsCancellationRequested)
                    return;

                try
                {
                    sender.Send(metricEvent);
                }
                catch (Exception error)
                {
                    OnError(error);
                }
            }
        }

        public Task WaitForIterationEnd() => iterationEnd.WaitAsync();

        private static DateTime Now => PreciseDateTime.UtcNow.UtcDateTime;

        private void Scrape(IEnumerable<IScrapableMetric> metrics, DateTime? scrapeTimestamp, CancellationToken cancellationToken)
        {
            metricEventsBuffer.Clear();
            scrapeTimestamp = scrapeTimestamp ?? Now;

            foreach (var metric in metrics)
            {
                if (cancellationToken.IsCancellationRequested)
                    return;

                try
                {
                    metricEventsBuffer.AddRange(metric.Scrape(scrapeTimestamp.Value));
                }
                catch (Exception error)
                {
                    OnError(error);
                }
            }
        }

        private static TimeSpan GetDelayToNextScrape(DateTime now, TimeSpan period)
            => period - TimeSpan.FromTicks(now.Ticks % period.Ticks);

        private static async Task<DateTime> WaitForTimestampRollover(DateTime threshold, CancellationToken cancellationToken)
        {
            while (true)
            {
                var scrapeTimestamp = Now;
                if (scrapeTimestamp >= threshold)
                    return scrapeTimestamp;

                await Task.Delay(RolloverWaitPause, cancellationToken).ConfigureAwait(false);
            }
        }

        private void OnError(Exception error)
        {
            try
            {
                errorCallback?.Invoke(error);
            }
            catch
            {
                // ignored
            }
        }
    }
}