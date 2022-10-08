using System;
using JetBrains.Annotations;

namespace Vostok.Commons.Time
{
    [PublicAPI]
    internal static class DateTimeOffsetArithmetics
    {
        public static DateTimeOffset Min(DateTimeOffset time1, DateTimeOffset time2)
        {
            return time1 <= time2 ? time1 : time2;
        }

        public static DateTimeOffset Max(DateTimeOffset time1, DateTimeOffset time2)
        {
            return time1 >= time2 ? time1 : time2;
        }
    }
}