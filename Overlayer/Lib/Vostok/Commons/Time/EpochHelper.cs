using System;
using JetBrains.Annotations;

namespace Vostok.Commons.Time
{
    [PublicAPI]
    internal static class EpochHelper
    {
        /// <summary>
        /// Common era epoch (January 1, 0001) expressed as a UTC <see cref="DateTime"/> instance.
        /// </summary>
        public static readonly DateTime CommonEraEpoch = DateTime.MinValue;

        /// <summary>
        /// Gregorian epoch (October 15, 1582) expressed as a UTC <see cref="DateTime"/> instance.
        /// </summary>
        public static readonly DateTime GregorianEpoch = new DateTime(1582, 10, 15, 0, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// Unix epoch (January 1, 1970) expressed as a UTC <see cref="DateTime"/> instance.
        /// </summary>
        public static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// Returns the amount of 100-ns ticks elapsed between <see cref="CommonEraEpoch"/> and given <paramref name="timestamp"/> expressed in UTC.
        /// </summary>
        public static long ToCommonEraTicks(DateTime timestamp)
            => timestamp.ToUniversalTime().Ticks;

        /// <summary>
        /// Returns the amount of 100-ns ticks elapsed between <see cref="GregorianEpoch"/> and given <paramref name="timestamp"/> expressed in UTC.
        /// </summary>
        public static long ToGregorianUtcTicks(DateTime timestamp)
            => timestamp.ToUniversalTime().Ticks - GregorianEpoch.Ticks;

        /// <summary>
        /// Returns the amount of 100-ns ticks elapsed between <see cref="UnixEpoch"/> and given <paramref name="timestamp"/> expressed in UTC.
        /// </summary>
        public static long ToUnixTimeUtcTicks(DateTime timestamp)
            => timestamp.ToUniversalTime().Ticks - UnixEpoch.Ticks;

        /// <summary>
        /// Returns a UTC <see cref="DateTime"/> expressed by given amount of 100-ns <paramref name="ticks"/> elapsed since <see cref="CommonEraEpoch"/>.
        /// </summary>
        public static DateTime FromCommonEraUtcTicks(long ticks)
            => new DateTime(ticks, DateTimeKind.Utc);

        /// <summary>
        /// Returns a UTC <see cref="DateTime"/> expressed by given amount of 100-ns <paramref name="ticks"/> elapsed since <see cref="GregorianEpoch"/>.
        /// </summary>
        public static DateTime FromGregorianUtcTicks(long ticks)
            => new DateTime(GregorianEpoch.Ticks + ticks, DateTimeKind.Utc);

        /// <summary>
        /// Returns a UTC <see cref="DateTime"/> expressed by given amount of 100-ns <paramref name="ticks"/> elapsed since <see cref="UnixEpoch"/>.
        /// </summary>
        public static DateTime FromUnixTimeUtcTicks(long ticks)
            => new DateTime(UnixEpoch.Ticks + ticks, DateTimeKind.Utc);

        /// <summary>
        /// Returns a UTC <see cref="DateTime"/> expressed by given amount of <paramref name="milliseconds"/> elapsed since <see cref="UnixEpoch"/>.
        /// </summary>
        public static DateTime FromUnixTimeMilliseconds(long milliseconds)
            => UnixEpoch + TimeSpan.FromMilliseconds(milliseconds);
    }
}