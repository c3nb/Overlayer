using System;
using System.Globalization;

namespace Vostok.Metrics.Helpers
{
    internal static class TimeSpanSerializer
    {
        public static string Serialize(TimeSpan value) =>
            value.ToString("G", CultureInfo.InvariantCulture);

        public static TimeSpan Deserialize(string input) =>
            TimeSpan.Parse(input, CultureInfo.InvariantCulture);
    }
}