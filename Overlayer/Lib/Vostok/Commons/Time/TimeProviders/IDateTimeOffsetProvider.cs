using System;
using JetBrains.Annotations;

namespace Vostok.Commons.Time.TimeProviders
{
    [PublicAPI]
    internal interface IDateTimeOffsetProvider
    {
        DateTimeOffset Now { get; }
    }
}