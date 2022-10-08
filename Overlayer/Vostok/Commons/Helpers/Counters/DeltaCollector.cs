using System;
using System.Threading;
using JetBrains.Annotations;

namespace Vostok.Commons.Helpers.Counters;

[PublicAPI]
internal class DeltaCollector
{
    private readonly Func<long> provider;
    private long previousValue;

    public DeltaCollector(Func<long> provider)
        => this.provider = provider;

    public long Collect()
    {
        var currentValue = provider();
        var difference = currentValue - previousValue;

        Interlocked.Exchange(ref previousValue, currentValue);

        return difference;
    }
}