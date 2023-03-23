using System;
using JetBrains.Annotations;

namespace Vostok.Commons.Time.TimeProviders
{
    [PublicAPI]
    internal class DateTimeOffsetProvider : IDateTimeOffsetProvider
    {
        public static readonly DateTimeOffsetProvider Instance = new DateTimeOffsetProvider();

        public DateTimeOffset Now => DateTimeOffset.Now;
    }
}