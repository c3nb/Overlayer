using System;
using Vostok.Metrics.Models;

namespace Vostok.Metrics.Primitives.Gauge
{
    /// <summary>
    /// <para>
    /// Gauge metric represents an arbitrary numeric value.
    /// </para>
    /// <para>
    /// The value of Gauge is observed every <see cref="GaugeConfig.ScrapePeriod"/> and saved to a permanent storage without any aggregation process.
    /// </para>
    /// </summary>
    /// <remarks>
    /// <para> To create a Gauge, use factory extensions for <see cref="IMetricContext"/>:</para>
    /// <list type="bullet">
    ///     <item><description><see cref="FuncGaugeFactoryExtensions"/> — gauges whose values come from external delegates.</description></item>
    ///     <item><description><see cref="MultiFuncGaugeFactoryExtensions"/> — gauges whose values come from external delegates in batches in form of <see cref="MetricDataPoint"/>s.</description></item>
    ///     <item><description><see cref="IntegerGaugeFactoryExtensions"/> — manually manipulated gauges with <see cref="long"/> values.</description></item>
    ///     <item><description><see cref="FloatingGaugeFactoryExtensions"/> — manually manipulated gauges with <see cref="double"/> values.</description></item>
    /// </list>
    /// <para>
    /// You can call <see cref="IDisposable.Dispose"/> to stop observing Gauge values.
    /// </para>
    /// </remarks>
    /// <example>
    /// <para>
    /// You can use Gauge to send system metrics (like CPU usage).
    /// Your app <see cref="FuncGaugeFactoryExtensions">creates</see> a Gauge.
    /// <code>
    /// var gauge = context.FuncGauge(
    ///     "cpu-usage",
    ///     GetCpuUsage,
    ///     new GaugeConfig { ScrapePeriod = TimeSpan.FromSeconds(10) });
    /// </code>
    /// Every 10 seconds <c>GetCpuUsage</c> is called and returned value is saved to a permanent storage.
    /// </para>
    /// </example>
    /// <example>
    /// <para>
    /// Another example is the number of concurrent requests to your service split by the identity of a client.
    /// <code>
    /// var gauge = context.IntegerGauge(
    ///     "concurrent-requests",
    ///     "client-id");
    ///
    /// 
    /// gauge.For("fat-service").Increment();
    /// ... // process request
    /// gauge.For("fat-service").Decrement(); 
    /// </code>
    /// </para>
    /// </example>
    // ReSharper disable once UnusedMember.Global
    internal static class GaugeDocumentation
    {
    }
}