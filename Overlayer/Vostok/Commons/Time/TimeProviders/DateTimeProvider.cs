using System;
using JetBrains.Annotations;

namespace Vostok.Commons.Time.TimeProviders
{
    [PublicAPI]
    internal class DateTimeProvider : IDateTimeProvider
    {
        public static readonly DateTimeProvider Instance = new DateTimeProvider();

        public DateTime UtcNow => DateTime.UtcNow;

        public DateTime Now => DateTime.Now;
    }
}