using JetBrains.Annotations;
using Vostok.Metrics.Models;

namespace Vostok.Metrics
{
    /// <summary>
    /// Names for some of well-recognized measurement <see cref="MetricEvent.Unit">units</see>.
    /// </summary>
    [PublicAPI]
    public static class WellKnownUnits
    {
        public const string None = null;
        public const string Percent1 = "percent1";
        public const string Percent100 = "percent100";

        public const string Ticks = "ticks";
        public const string Nanoseconds = "nanoseconds";
        public const string Microseconds = "microseconds";
        public const string Milliseconds = "milliseconds";
        public const string Seconds = "seconds";
        public const string Minutes = "minutes";
        public const string Hours = "hours";
        public const string Days = "days";

        public const string OpsPerSecond = "ops" + PerSecondSuffix;
        public const string OpsPerMinute = "ops" + PerMinuteSuffix;

        public const string Bytes = "bytes";
        public const string Kilobytes = "kilobytes";
        public const string Megabytes = "megabytes";
        public const string Gigabytes = "gigabytes";
        public const string Terabytes = "terabytes";
        public const string Petabytes = "petabytes";
        public const string Exabytes = "exabytes";

        public const string Bits = "bits";
        public const string Kilobits = "kilobits";
        public const string Megabits = "megabits";
        public const string Gigabits = "gigabits";
        public const string Terabits = "terabits";
        public const string Petabits = "petabits";
        public const string Exabits = "exabits";

        public const string BytesPerSecond = Bytes + PerSecondSuffix;
        public const string KilobytesPerSecond = Kilobytes + PerSecondSuffix;
        public const string MegabytesPerSecond = Megabytes + PerSecondSuffix;
        public const string GigabytesPerSecond = Gigabytes + PerSecondSuffix;
        public const string TerabytesPerSecond = Terabytes + PerSecondSuffix;
        public const string PetabytesPerSecond = Petabytes + PerSecondSuffix;
        public const string ExabytesPerSecond = Exabytes + PerSecondSuffix;

        public const string BitsPerSecond = Bits + PerSecondSuffix;
        public const string KilobitsPerSecond = Kilobits + PerSecondSuffix;
        public const string MegabitsPerSecond = Megabits + PerSecondSuffix;
        public const string GigabitsPerSecond = Gigabits + PerSecondSuffix;
        public const string TerabitsPerSecond = Terabits + PerSecondSuffix;
        public const string PetabitsPerSecond = Petabits + PerSecondSuffix;
        public const string ExabitsPerSecond = Exabits + PerSecondSuffix;

        private const string PerSecondSuffix = "/second";
        private const string PerMinuteSuffix = "/minute";
    }
}