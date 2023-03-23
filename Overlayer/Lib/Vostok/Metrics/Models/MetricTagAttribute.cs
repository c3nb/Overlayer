using System;
using JetBrains.Annotations;
using Vostok.Metrics.Grouping;

namespace Vostok.Metrics.Models
{
    /// <summary>
    /// <para><see cref="MetricTagAttribute"/> marks a public property of the model object as a source of metric tag for <see cref="IMetricGroup{TFor,TMetric}"/>.</para>
    /// <para>User has to explicitly specify order in which properties will be converted to tags by providing a numeric <see cref="Order"/> value.</para>
    /// <para>Properties are then enumerated in ascending order based on <see cref="Order"/> values of the attributes.</para>
    /// </summary>
    [PublicAPI]
    [AttributeUsage(AttributeTargets.Property)]
    public class MetricTagAttribute : Attribute
    {
        public MetricTagAttribute(int order)
            => Order = order;

        public int Order { get; }
    }
}