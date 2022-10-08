using System;
using JetBrains.Annotations;
using Vostok.Metrics.Models;

namespace Vostok.Metrics.Senders
{
    /// <summary>
    /// Prints incoming <see cref="MetricEvent"/>s to system console.
    /// </summary>
    [PublicAPI]
    public class ConsoleMetricEventSender : TextMetricEventSender
    {
        public ConsoleMetricEventSender(MetricEventPrintFormat format)
            : base(Console.Out.WriteLine, format)
        {
        }
    }
}
