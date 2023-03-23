using System;
using JetBrains.Annotations;

namespace Vostok.Metrics.Primitives.Timer
{
    internal static class TimeValuesConverter
    {
        public static double ConvertOrThrow(TimeSpan value, [CanBeNull] string unit)
        {
            switch (unit)
            {
                case WellKnownUnits.Ticks:
                    return value.Ticks;

                case WellKnownUnits.Microseconds:
                    return value.TotalMilliseconds * 1000d;

                case WellKnownUnits.Milliseconds:
                    return value.TotalMilliseconds;

                case WellKnownUnits.Seconds:
                    return value.TotalSeconds;

                case WellKnownUnits.Minutes:
                    return value.TotalMinutes;

                case WellKnownUnits.Hours:
                    return value.TotalHours;

                case WellKnownUnits.Days:
                    return value.TotalDays;
            }

            throw new ArgumentException($"Cannot convert a TimeSpan value to '{unit}' unit.");
        }
    }
}