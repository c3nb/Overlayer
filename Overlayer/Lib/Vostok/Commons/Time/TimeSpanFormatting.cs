using System;
using System.Globalization;
using JetBrains.Annotations;

namespace Vostok.Commons.Time
{
    [PublicAPI]
    internal static class TimeSpanFormatting
    {
        private const long TicksPerMicrosecond = TimeSpan.TicksPerMillisecond / 1000;

        public static string ToPrettyString(this TimeSpan time, bool useShortUnitNames = false)
        {
            if (time == TimeSpan.MaxValue)
                return "infinite";

            if (time.TotalDays >= 1)
                return time.TotalDays.ToString("0.###", CultureInfo.InvariantCulture) + (useShortUnitNames ? "d" : " days");

            if (time.TotalHours >= 1)
                return time.TotalHours.ToString("0.###", CultureInfo.InvariantCulture) + (useShortUnitNames ? "h" : " hours");

            if (time.TotalMinutes >= 1)
                return time.TotalMinutes.ToString("0.###", CultureInfo.InvariantCulture) + (useShortUnitNames ? "m" : " minutes");

            if (time.TotalSeconds >= 1)
                return time.TotalSeconds.ToString("0.###", CultureInfo.InvariantCulture) + (useShortUnitNames ? "s" : " seconds");

            if (time.TotalMilliseconds >= 1)
                return time.TotalMilliseconds.ToString("0.###", CultureInfo.InvariantCulture) + (useShortUnitNames ? "ms" : " milliseconds");

            var totalMicroseconds = (double)time.Ticks / TicksPerMicrosecond;
            if (totalMicroseconds >= 1)
                return totalMicroseconds.ToString("0.###", CultureInfo.InvariantCulture) + (useShortUnitNames ? "us" : " microseconds");

            return time.ToString();
        }
    }
}