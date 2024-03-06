using Overlayer.Core;
using Overlayer.Tags.Attributes;
using System;

namespace Overlayer.Tags
{
    public static class Time
    {
        [Tag(NotPlaying = true)]
        public static int Year() => FastDateTime.Now.Year;
        [Tag(NotPlaying = true)]
        public static int Month() => FastDateTime.Now.Month;
        [Tag(NotPlaying = true)]
        public static int Day() => FastDateTime.Now.Day;
        [Tag(NotPlaying = true)]
        public static double Days() => TimeSpan.FromTicks(FastDateTime.Now.Ticks).TotalDays;
        [Tag(NotPlaying = true)]
        public static int Hour() => FastDateTime.Now.Hour;
        [Tag(NotPlaying = true)]
        public static double Hours() => TimeSpan.FromTicks(FastDateTime.Now.Ticks).TotalHours;
        [Tag(NotPlaying = true)]
        public static int Minute() => FastDateTime.Now.Minute;
        [Tag(NotPlaying = true)]
        public static double Minutes() => TimeSpan.FromTicks(FastDateTime.Now.Ticks).TotalMinutes;
        [Tag(NotPlaying = true)]
        public static int Second() => FastDateTime.Now.Second;
        [Tag(NotPlaying = true)]
        public static double Seconds() => TimeSpan.FromTicks(FastDateTime.Now.Ticks).TotalSeconds;
        [Tag(NotPlaying = true)]
        public static int MilliSecond() => FastDateTime.Now.Millisecond;
        [Tag(NotPlaying = true)]
        public static double MilliSeconds() => TimeSpan.FromTicks(FastDateTime.Now.Ticks).TotalMilliseconds;
        public static void Reset() { }
    }
}
