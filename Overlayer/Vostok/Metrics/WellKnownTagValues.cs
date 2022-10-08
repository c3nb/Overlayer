using JetBrains.Annotations;

namespace Vostok.Metrics
{
    /// <summary>
    /// Values for some of the <see cref="WellKnownTagKeys"/>.
    /// </summary>
    [PublicAPI]
    public static class WellKnownTagValues
    {
        /// <summary>
        /// A special value used for <see cref="WellKnownTagKeys.LowerBound"/> tag.
        /// </summary>
        public const string NegativeInfinity = "-Inf";

        /// <summary>
        /// A special value used for <see cref="WellKnownTagKeys.UpperBound"/> tag.
        /// </summary>
        public const string PositiveInfinity = "+Inf";

        /// <summary>
        /// One of the values for <see cref="WellKnownTagKeys.Aggregate"/> tag.
        /// </summary>
        public const string AggregateCount = "count";

        /// <inheritdoc cref="AggregateCount"/>
        public const string AggregateAverage = "avg";

        /// <inheritdoc cref="AggregateCount"/>
        public const string AggregateP50 = "p50";

        /// <inheritdoc cref="AggregateCount"/>
        public const string AggregateP75 = "p75";

        /// <inheritdoc cref="AggregateCount"/>
        public const string AggregateP95 = "p95";

        /// <inheritdoc cref="AggregateCount"/>
        public const string AggregateP99 = "p99";

        /// <inheritdoc cref="AggregateCount"/>
        public const string AggregateP999 = "p999";
    }
}