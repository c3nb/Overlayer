using System;
using JetBrains.Annotations;

namespace Vostok.Commons.Time
{
    [PublicAPI]
    internal static class DateTimeArithmetics
    {
        public static DateTime Min(DateTime time1, DateTime time2)
        {
            return time1 <= time2 ? time1 : time2;
        }

        public static DateTime Max(DateTime time1, DateTime time2)
        {
            return time1 >= time2 ? time1 : time2;
        }
    }
}