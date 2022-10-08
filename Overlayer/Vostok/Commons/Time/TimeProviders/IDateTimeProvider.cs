using System;
using JetBrains.Annotations;

namespace Vostok.Commons.Time.TimeProviders
{
    [PublicAPI]
    internal interface IDateTimeProvider
    {
        DateTime UtcNow { get; }

        DateTime Now { get; }
    }
}