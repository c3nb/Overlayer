using System;
using JetBrains.Annotations;

namespace Vostok.Commons.Time
{
    [PublicAPI]
    internal static class TimeSpanArithmetics
    {
        public static TimeSpan Multiply(this TimeSpan time, double multiplier)
        {
            return TimeSpan.FromTicks((long)(time.Ticks * multiplier));
        }

        public static TimeSpan Divide(this TimeSpan time, double divisor)
        {
            return TimeSpan.FromTicks((long)(time.Ticks / divisor));
        }

        public static TimeSpan Abs(this TimeSpan time)
        {
            return time.Ticks >= 0 ? time : time.Negate();
        }

        public static TimeSpan Min(TimeSpan time1, TimeSpan time2)
        {
            return time1 <= time2 ? time1 : time2;
        }

        public static TimeSpan Min(TimeSpan time1, TimeSpan time2, TimeSpan time3)
        {
            return Min(time1, Min(time2, time3));
        }

        public static TimeSpan Max(TimeSpan time1, TimeSpan time2)
        {
            return time1 >= time2 ? time1 : time2;
        }

        public static TimeSpan Max(TimeSpan time1, TimeSpan time2, TimeSpan time3)
        {
            return Max(time1, Max(time2, time3));
        }

        /// <param name="min">Minimum time border</param>
        /// <param name="max">Maximum time border</param>
        /// <returns>If <paramref name="time"/> in [<paramref name="min"/>, <paramref name="max"/>] returns time, otherwise returns closest to this segment value (<paramref name="min"/> or <paramref name="max"/>)</returns>
        /// <exception cref="ArgumentException">throws if <paramref name="min"/> > <paramref name="max"/></exception>
        public static TimeSpan Clamp(this TimeSpan time, TimeSpan min, TimeSpan max)
        {
            if (min > max)
                throw new ArgumentException($"Expected min <= max, but actually min = {min}, max = {max}.");
            return Max(Min(time, max), min);
        }

        /// <summary>
        /// Returns <paramref name="baseValue"/> reduced by the minimum of <paramref name="absoluteValueToCut"/> and <paramref name="relativeValueToCut"/> multiplied by <paramref name="baseValue"/>.
        /// </summary>
        public static TimeSpan Cut(this TimeSpan baseValue, TimeSpan absoluteValueToCut, double relativeValueToCut)
            => baseValue - Min(absoluteValueToCut, baseValue.Multiply(relativeValueToCut));
    }
}